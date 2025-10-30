using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using invoice.Models; // Assurez-vous d'avoir ce using pour accéder à Facture, Examen, FactureExamen
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class FactureDocument : IDocument
{
    private string imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "img", "headerPDF.png");
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
            page.Margin(30);

            page.Header().AlignLeft().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Element(ComposeFooter);
        });
    }

    // ----------------------------------------------------
    // COMPOSANTS DE LA PAGE
    // ----------------------------------------------------

    public void ComposeHeader(IContainer container)
    {
        container
            .Height(180)
            .PaddingVertical(10)
            .Column(column =>
            {
                column.Spacing(10);
                column.Item().Image(imagePath).FitWidth();

                column.Item().PaddingVertical(10).Row(row =>
                {
                    row.RelativeItem(3).AlignLeft().AlignBottom().Column(column =>
                    {
                        column.Item().Text("Facture").FontSize(20).SemiBold();
                        column.Item().Text($"Réf.  : {_facture.Reference}").FontSize(11);
                    });

                    row.RelativeItem(2).AlignMiddle().AlignRight().Column(column =>
                    {
                        column.Item().Text($"Date : {_facture.Created_at:dd/MM/yyyy}").FontSize(11).SemiBold();
                    });
                });

            });
    }

    public void ComposeFooter(IContainer container)
    {
        container
            .Height(70)
            .AlignBottom()
            .Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Spacing(10);
                    column.Item()
                    .Container()
                        .Height(4)
                        .Width(470)
                        .Background(Colors.Green.Lighten2)
                        .AlignCenter();

                    column.Item()
                    .Text(x =>
                    {
                        x.AlignCenter();
                        x.Span("S.A.R.L au capital de 2 000 000 de FCFA siège social à owendo, Akournam II non loin du carrefour. NIF: 2024 0102 1465; w: ;RCCM: GA-LBV-01-2024-B12-01194. BP: 7441").FontSize(12).FontColor(Colors.Blue.Lighten2).SemiBold();
                    });

                    column.Item()
                    .Text(x =>
                    {
                        x.AlignRight();
                        x.Span("Page ").FontSize(9);
                        x.CurrentPageNumber().FontSize(9);
                        x.Span(" / ").FontSize(9);
                        x.TotalPages().FontSize(9);
                    });
                });
            });
    }

    public void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(100);

            // 1. Informations du Patient
            column.Item().Element(ComposePatientInfo);
            // 2. Tableau Central des Examens
            column.Item().Element(ComposeInvoiceTable);

            column.Spacing(50);
            // 3. Totaux et Résumé Financier
            column.Item().Element(ComposeTotals);
        });
    }

    public void ComposePatientInfo(IContainer container)
    {
        DateTime dateNaissance = (DateTime)_patient?.DateOfBirth!;
        int age = CalculerAge(dateNaissance);
        container.Background(Colors.Grey.Lighten4).Padding(10).Column(column =>
        {
            column.Item().Row(row =>
            { 
                row.ConstantItem(180).Text("Patient :").SemiBold().FontSize(9);
                row.Spacing(15);
                row.ConstantItem(150).Text("Date de naissance :").SemiBold().FontSize(9);
            });
            column.Spacing(3);
            column.Item().Row(row =>
            {
                row.ConstantItem(180).Text($"{_patient.FirstName ?? "N/A"} {_patient.LastName ?? "N/A"}").FontSize(12);
                row.Spacing(15);
                row.ConstantItem(100).Text($"{_patient.DateOfBirth:dd/MM/yyyy}").FontSize(12);
            });

            column.Item().PaddingTop(5).Row(row =>
            {
                row.ConstantItem(30).Text("Age :").SemiBold().FontSize(9);
                row.ConstantItem(20).PaddingLeft(5).Text($"{age}").SemiBold().FontSize(10);
            });
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
                column.ConstantColumn(40);  // Qté
                column.ConstantColumn(80);  // Prix Unitaire
                column.ConstantColumn(100);  // Montant HT
            });
            // Définition des colonnes

            // En-têtes du tableau
            table.Header(header =>
            {
                header.Cell().BorderHorizontal(1).BorderColor(Colors.Black).BorderVertical(1).BorderColor(Colors.Black).Padding(8).Text("Référence").Bold();
                header.Cell().BorderHorizontal(1).BorderColor(Colors.Black).BorderVertical(1).BorderColor(Colors.Black).AlignLeft().Padding(8).Text("Désignation").Bold();
                header.Cell().BorderHorizontal(1).BorderColor(Colors.Black).BorderVertical(1).BorderColor(Colors.Black).AlignRight().Padding(8).Text("Qté").Bold();
                header.Cell().BorderHorizontal(1).BorderColor(Colors.Black).BorderVertical(1).BorderColor(Colors.Black).AlignRight().Padding(8).Text("Px Unitaire").Bold();
                header.Cell().BorderHorizontal(1).BorderColor(Colors.Black).BorderVertical(1).BorderColor(Colors.Black).AlignRight().Padding(8).Text("Montant HT").Bold();
            });

            // Corps du tableau (Lignes d'examens)
            foreach (var line in lines)
            {
                var examen = line.Examen;
                decimal totalHtLigne = examen!.Price * line.Qte;

                table.Cell().BorderHorizontal(1).BorderColor(Colors.Black).BorderVertical(1).BorderColor(Colors.Black).Padding(6).Text(examen.Reference.ToString());
                table.Cell().BorderHorizontal(1).BorderColor(Colors.Black).BorderVertical(1).BorderColor(Colors.Black).Padding(6).Text(examen.ExamenName);
                table.Cell().BorderHorizontal(1).BorderColor(Colors.Black).BorderVertical(1).BorderColor(Colors.Black).AlignRight().Padding(6).Text(line.Qte.ToString());
                table.Cell().BorderHorizontal(1).BorderColor(Colors.Black).BorderVertical(1).BorderColor(Colors.Black).Padding(6).AlignRight().Text($"{examen.Price:N2}");
                table.Cell().BorderHorizontal(1).BorderColor(Colors.Black).BorderVertical(1).BorderColor(Colors.Black).Padding(6).AlignRight().Text($"{totalHtLigne:N2}");
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
                row.ConstantItem(80).AlignRight().Text($"{patientShare:C}").SemiBold().FontSize(9).FontColor(Colors.Blue.Darken2);
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
                row.ConstantItem(90).AlignRight().Text($"{amountDue:C}").ExtraBold().FontSize(11).FontColor(Colors.Red.Darken2);
            });
        });
    }
    public static int CalculerAge(DateTime dateDeNaissance)
    {
        int age = DateTime.Today.Year - dateDeNaissance.Year;
        if (DateTime.Today.Month < dateDeNaissance.Month || (DateTime.Today.Month == dateDeNaissance.Month && DateTime.Today.Day < dateDeNaissance.Day))
        {
            age--;
        }
        return age;
    }
}