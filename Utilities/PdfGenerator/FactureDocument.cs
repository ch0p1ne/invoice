using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using invoice.Models; // Assurez-vous d'avoir ce using pour accéder à Facture, Examen, FactureExamen
using System;
using System.Collections.Generic;
using System.Linq;

public class FactureDocument : IDocument
{
    private readonly Facture _facture;
    private readonly Patient _patient;

    // La propriété FacturesExamens de Facture sera utilisée pour les lignes

    public FactureDocument(Facture facture, Patient patient)
    {
        _facture = facture ?? throw new ArgumentNullException(nameof(facture));

        _patient = patient ?? throw new ArgumentNullException(nameof(patient));
    }

    // ----------------------------------------------------
    // METADATA
    // ----------------------------------------------------
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    // ----------------------------------------------------
    // MISE EN PAGE GLOBALE (Header, Footer, Content)
    // ----------------------------------------------------
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    // ----------------------------------------------------
    // COMPOSANTS DE LA PAGE
    // ----------------------------------------------------

    public void ComposeHeader(IContainer container)
    {
        container
            .Height(60)
            .Background(Colors.Grey.Lighten3) // Gris clair
            .PaddingVertical(10)
            .Row(row =>
            {
                row.RelativeItem(3).Column(column =>
                {
                    column.Item().Text("Facture").FontSize(20).SemiBold();
                    column.Item().Text($"Réf. : {_facture.Reference}").FontSize(11);
                });

                row.RelativeItem(2).AlignRight().Column(column =>
                {
                    column.Item().Text("Clinique CLIMA-G").SemiBold();
                    column.Item().Text("Adresse : Aéorodrome").FontSize(9);
                    column.Item().Text($"Date : {_facture.Created_at:dd/MM/yyyy}").FontSize(9);
                });
            });
    }

    public void ComposeFooter(IContainer container)
    {
        container
            .Height(30)
            .Background(Colors.Grey.Lighten3) // Gris clair
            .AlignMiddle()
            .Text(x =>
            {
                x.AlignRight();
                x.Span("Page ").FontSize(9);
                x.CurrentPageNumber().FontSize(9);
                x.Span(" / ").FontSize(9);
                x.TotalPages().FontSize(9);
            });
    }

    public void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(25);

            // 1. Informations du Patient
            column.Item().Element(ComposePatientInfo);

            // 2. Tableau Central des Examens
            column.Item().Element(ComposeInvoiceTable);

            // 3. Totaux et Résumé Financier
            column.Item().Element(ComposeTotals);
        });
    }

    public void ComposePatientInfo(IContainer container)
    {
        container.Background(Colors.Grey.Lighten4).Padding(10).Column(column =>
        {
            column.Item().Text("Patient :").SemiBold().FontSize(12);
            column.Item().Text($"{_patient.FirstName ?? "N/A"} {_patient.LastName ?? "N/A"}").FontSize(10);
        });
    }

    public void ComposeInvoiceTable(IContainer container)
    {
        var lines = _facture.FacturesExamens;

        container.Table(table =>
        {
            table.ColumnsDefinition(column =>
            {
                column.ConstantColumn(70);  // Réf Examen
                column.RelativeColumn(3);   // Description
                column.ConstantColumn(50);  // Qté
                column.ConstantColumn(70);  // Prix Unitaire
                column.ConstantColumn(70);  // Total HT
            });
            // Définition des colonnes

            // En-têtes du tableau
            table.Header(header =>
            {
                header.Cell().PaddingVertical(5).Text("Réf").Bold();
                header.Cell().PaddingVertical(5).Text("Désignation").Bold();
                header.Cell().Text("Qté").Bold();
                header.Cell().AlignRight().Text("P.U. HT").Bold();
                header.Cell().AlignRight().Text("Total HT").Bold();
            });

            // Corps du tableau (Lignes d'examens)
            foreach (var line in lines)
            {
                var examen = line.Examen;
                decimal totalHtLigne = examen.Price * line.Qte;

                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(examen.Reference.ToString());
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(examen.ExamenName);
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(line.Qte.ToString());
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{examen.Price:N2}");
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"{totalHtLigne:N2}");
            }
        });
    }

    public void ComposeTotals(IContainer container)
    {
        // Calcul des totaux basiques (assurez-vous que TotalAmountHT est peuplé dans l'entité)
        decimal totalHT = _facture.TotalAmountHT ?? 0m;
        decimal totalTVA = totalHT * (_facture.Tva);
        decimal totalTTC = totalHT + totalTVA;
        decimal patientShare = totalTTC * _facture.PatientPercent;
        decimal amountPaid = _facture.AmountPaid ?? 0m;
        decimal amountDue = totalTTC - amountPaid;

        container.AlignRight().PaddingRight(5).Column(column =>
        {
            column.Spacing(5);

            // Total HT
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignRight().Text("TOTAL HT :").SemiBold();
                row.ConstantItem(80).AlignRight().Text($"{totalHT:C}").SemiBold();
            });

            // TVA
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignRight().Text($"TVA ({_facture.Tva * 100:N0}%) :").SemiBold();
                row.ConstantItem(80).AlignRight().Text($"{totalTVA:C}");
            });

            // TOTAL TTC
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignRight().Text("TOTAL TTC :").ExtraBold();
                row.ConstantItem(80).AlignRight().Text($"{totalTTC:C}").ExtraBold();
            });

            // Part Patient
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignRight().Text($"Part Patient ({_facture.PatientPercent * 100:N0}%) :").SemiBold().FontSize(11).FontColor(Colors.Blue.Darken2);
                row.ConstantItem(80).AlignRight().Text($"{patientShare:C}").SemiBold().FontSize(11).FontColor(Colors.Blue.Darken2);
            });

            // Montant Payé
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignRight().Text("Acompte/Payé :").SemiBold();
                row.ConstantItem(80).AlignRight().Text($"{amountPaid:C}").SemiBold().FontColor(Colors.Green.Darken2);
            });

            // Reste à Payer
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignRight().Text("Reste à Payer :").ExtraBold().FontSize(14);
                row.ConstantItem(80).AlignRight().Text($"{amountDue:C}").ExtraBold().FontSize(14).FontColor(Colors.Red.Darken2);
            });
        });
    }
}