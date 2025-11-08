using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using invoice.Models; // Assurez-vous d'avoir ce using pour accéder à Facture, Examen, FactureExamen
using System.IO;
using Humanizer;

public class FactureDocument : IDocument
{
    private string imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "img", "headerPDF.png");
    private readonly Facture _facture;
    private readonly Patient _patient;
    private readonly User _user;
    private int netAPayer;
    

    // La propriété FacturesExamens de Facture sera utilisée pour les lignes

    public FactureDocument(Facture facture, Patient patient, User user)
    {
        
        _facture = facture ?? throw new ArgumentNullException(nameof(facture));

        _patient = patient ?? throw new ArgumentNullException(nameof(patient));

        _user = user ?? throw new ArgumentNullException(nameof(user));
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
            .Height(130)
            .AlignBottom()
            .Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Spacing(10);
                    column.Item().Text(x =>
                    {
                        x.AlignCenter();
                        x.Span($"Arrété la présente facture à la somme de :\n{NumberToWordsExtension.ToWords(netAPayer)} FCFA").FontSize(11).Light();
                    });
                    column.Item().Text(x =>
                    {
                        x.AlignLeft();
                        x.Span($"{_user.Account_name}").FontSize(7).Light();
                    });
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
                        x.Span("S.A.R.L au capital de 2 000 000 de FCFA siège social à owendo, Akournam II non loin du carrefour. NIF: 2024 0102 1465 w: RCCM: GA-LBV-01-2024-B12-01194: BP: 7441").FontSize(12).FontColor(Colors.Blue.Darken2).SemiBold();
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
        DateTime dateNaissance = (DateTime)_patient?.DateOfBirth;
        int age = CalculerAge(dateNaissance);
        container.Background(Colors.Grey.Lighten4).Padding(10).Column(column =>
        {
            column.Item().Row(row =>
            { 
                row.ConstantItem(210).Text("Patient :").NormalWeight().FontSize(11);
                row.Spacing(2);
                row.ConstantItem(150).Text("Date de naissance :").NormalWeight().FontSize(11);
                row.ConstantItem(150).Text("Mode de paiement :").NormalWeight().FontSize(11);
            });
            column.Spacing(3);
            column.Item().Row(row =>
            {
                row.ConstantItem(210).Text($"{_patient.FirstName ?? "N/A"} {_patient.LastName ?? "N/A"}").FontSize(11).SemiBold();
                row.Spacing(2);
                row.ConstantItem(150).Text($"{_patient.DateOfBirth:dd/MM/yyyy}").FontSize(12).SemiBold();
                row.ConstantItem(150).Text($"{_facture.PaymentMethod}").FontSize(12).SemiBold();
            });

            column.Item().PaddingTop(5).Row(row =>
            {
                row.ConstantItem(35).Text("Age :").NormalWeight().FontSize(11);
                row.ConstantItem(35).PaddingLeft(5).Text($"{age}").SemiBold().FontSize(10);
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
                header.Cell().BorderHorizontal(1).Background(Colors.Grey.Lighten3).BorderColor(Colors.Grey.Lighten3).BorderVertical(1).BorderColor(Colors.Black).Padding(8).Text("Référence").Bold();
                header.Cell().BorderHorizontal(1).Background(Colors.Grey.Lighten3).BorderColor(Colors.Grey.Lighten3).BorderVertical(1).BorderColor(Colors.Black).AlignLeft().Padding(8).Text("Désignation").Bold();
                header.Cell().BorderHorizontal(1).Background(Colors.Grey.Lighten3).BorderColor(Colors.Grey.Lighten3).BorderVertical(1).BorderColor(Colors.Black).AlignRight().Padding(8).Text("Qté").Bold();
                header.Cell().BorderHorizontal(1).Background(Colors.Grey.Lighten3).BorderColor(Colors.Grey.Lighten3).BorderVertical(1).BorderColor(Colors.Black).AlignRight().Padding(8).Text("Px Unitaire").Bold();
                header.Cell().BorderHorizontal(1).Background(Colors.Grey.Lighten3).BorderColor(Colors.Grey.Lighten3).BorderVertical(1).BorderColor(Colors.Black).AlignRight().Padding(8).Text("Montant HT").Bold();
            });

            // Corps du tableau (Lignes d'examens)
            foreach (var line in lines)
            {
                var examen = line.Examen;
                decimal totalHtLigne = examen!.Price * line.Qte;

                table.Cell().BorderVertical(1).BorderColor(Colors.Grey.Lighten3).Padding(6).Text(examen.Reference.ToString()).FontSize(9);
                table.Cell().BorderVertical(1).BorderColor(Colors.Grey.Lighten3).Padding(6).Text(examen.ExamenName).FontSize(9);
                table.Cell().BorderVertical(1).BorderColor(Colors.Grey.Lighten3).AlignRight().Padding(6).Text(line.Qte.ToString()).FontSize(9);
                table.Cell().BorderVertical(1).BorderColor(Colors.Grey.Lighten3).Padding(6).AlignRight().Text($"{examen.Price:N2}").FontSize(9);
                table.Cell().BorderVertical(1).BorderColor(Colors.Grey.Lighten3).Padding(6).AlignRight().Text($"{totalHtLigne:N2}").FontSize(9);
            }
        });
    }

    public void ComposeTotals(IContainer container)
    {
        // Calcul des totaux basiques (assurez-vous que TotalAmountHT est peuplé dans l'entité)
        decimal totalHT = _facture.TotalAmountHT ?? 0m;
        decimal totalTVA = totalHT * (_facture.Tva);
        double remise = (double)(_facture.DiscountPercent ?? 0);
        double totalTTC = (double)totalHT + (double)totalTVA;
        double totalWithRemise = (double)totalHT * remise;
        double netAPayer = totalTTC - totalWithRemise;
        double patientShare = netAPayer * (double)_facture.PatientPercent;
        decimal amountPaid = _facture.AmountPaid ?? 0m;
        double amountDue = netAPayer - (double)amountPaid;
        this.netAPayer = (int) amountDue;


        container.AlignRight().PaddingRight(5).Column(column =>
        {
            column.Spacing(5);

            // Total HT
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignRight().PaddingRight(10).Text("TOTAL HT :").SemiBold().FontSize(11);
                row.ConstantItem(80).AlignRight().Text($"{totalHT:C}").SemiBold();
            });

            // Remise
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignRight().PaddingRight(10).Text($"Remise ({remise:P0}) :").SemiBold().FontSize(11).FontColor(Colors.Blue.Darken2);
                row.ConstantItem(80).AlignRight().Text($"{totalWithRemise:C}").SemiBold().FontSize(10).FontColor(Colors.Blue.Darken2);
            });
            // Net à payer
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignRight().PaddingRight(10).Text($"Net à payer :").SemiBold().FontSize(11);
                row.ConstantItem(80).AlignRight().Text($"{netAPayer:C}").SemiBold().FontSize(10);
            });
            
            // Montant Payé
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignRight().PaddingRight(10).Text("Avance :").SemiBold().FontSize(11);
                row.ConstantItem(80).AlignRight().Text($"{amountPaid:C}").SemiBold().FontSize(10);
            });

            // Reste à Payer
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignRight().PaddingRight(10).Text("Reste à Payer :").ExtraBold().FontSize(13);
                row.ConstantItem(90).AlignRight().Text($"{amountDue:C}").ExtraBold().FontColor(Colors.Red.Darken2);
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