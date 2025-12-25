using Humanizer;
using invoice.Models; // Assurez-vous d'avoir ce using pour accéder à Facture, Examen, FactureExamen
using invoice.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Windows.Shapes;

public class FactureDocument : IDocument
{
    private const int CONST_AVAILABLE_CONTENT_SIZE = 15;
    private string imagePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "img", "headerPDF.png");
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
            page.DefaultTextStyle(x =>
            x.FontFamily("Times New Roman"));

            page.Header().AlignCenter().Element(ComposeHeader);
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
            .Height(105)
            .Column(column =>
            {
                column.Spacing(0);
                column.Item().PaddingTop(-20).Image(imagePath).FitWidth();

                column.Item().Row(row =>
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
            .Height(100)
            .AlignBottom()
            .Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Spacing(5);
                    column.Item().Text(x =>
                    {
                        x.AlignCenter();
                        x.Span($"Arrété la présente facture à la somme de : {NumberToWordsExtension.ToWords(netAPayer)} FCFA").FontSize(11).Light();
                    });
                    column.Item().Text(x =>
                    {
                        x.AlignLeft();
                        x.Span($"{_user.Account_name}").FontSize(7).Light();
                    });
                    column.Item()
                    .Container()
                        .Height(4)
                        .Width(535)
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
            column.Spacing(30);

            // 1. Informations du Patient
            column.Item().Element(ComposePatientInfo);
            // 2. Tableau Central des Examens
            column.Item().Element(ComposeInvoiceTable);
        });
    }

    public void ComposePatientInfo(IContainer container)
    {
        DateTime dateNaissance = (DateTime)_patient.DateOfBirth!;
        int age = CalculerAge(dateNaissance);
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.ConstantItem(55).PaddingTop(15).Text("Patient :").SemiBold().FontSize(14);
                row.Spacing(15);
                row.RelativeItem(3).AlignBottom().Text($"{_patient.LastName ?? "N/A"} {_patient.FirstName ?? "N/A"}").FontSize(12).NormalWeight();
            });
            column.Spacing(2);
            column.Item().Row(row =>
            {
                row.ConstantItem(120).PaddingTop(3).Text("Date de naissance :").SemiBold().FontSize(14);
                row.Spacing(10);
                row.ConstantItem(100).AlignBottom().Text($"{_patient.DateOfBirth:dd/MM/yyyy}").FontSize(12).NormalWeight();
                row.ConstantItem(32).AlignBottom().Text("Age :").SemiBold().FontSize(14);
                row.ConstantItem(70).AlignBottom().PaddingLeft(5).Text($"{age} ans").NormalWeight().FontSize(12);
            });

            column.Item().Row(row =>
            {


                row.ConstantItem(50).PaddingTop(3).Text("Nº Tél :").SemiBold().FontSize(14);
                row.Spacing(10);
                row.ConstantItem(170).AlignBottom().Text($"{_patient.PhoneNumber} / {_patient.PhoneNumber2}").NormalWeight().FontSize(12);
                row.ConstantItem(60).PaddingTop(3).Text("Adresse :").SemiBold().FontSize(14);
                row.RelativeItem(170).AlignBottom().Text($"{_patient.Address}").NormalWeight().FontSize(12);
            });
        });
    }

    public void ComposeInvoiceTable(IContainer container)
    {
        var lines = _facture.FacturesExamens;
        int AvailableContentSize = CONST_AVAILABLE_CONTENT_SIZE;
        // Calcul des totaux basiques (assurez-vous que TotalAmountHT est peuplé dans l'entité)
        decimal totalHT = _facture.TotalAmountHT;
        double remisePercent =_facture.DiscountPercent;
        double remiseFlat = _facture.DiscountFlat;
        double CSS = (double)totalHT * (double)_facture.Css;
        double TPS = (double)totalHT * (double)_facture.TPS;
        double totalTTC = (double)totalHT + (_facture.DiscountPercent * (double)totalHT) + _facture.DiscountFlat + CSS + TPS;
        double totalWithRemise;
        if( remisePercent > 0)
        {
            totalWithRemise = totalTTC - (totalTTC * remisePercent);
        }
        else if(remiseFlat > 0)
        {
            totalWithRemise = totalTTC - remiseFlat;
        }
        else
        {
            totalWithRemise = totalTTC;
        }
        double netAPayer = totalWithRemise;  
        double patientShare = netAPayer * (double)_facture.PatientPercent;
        decimal amountPaid = _facture.AmountPaid ?? 0m;
        double amountDue = netAPayer - (double)amountPaid;
        this.netAPayer = (int)netAPayer;

        container
            .PaddingTop(-15)
            .Border(1) // Bordure globale autour du tableau
            .BorderColor(Colors.Black)
            .Table(table =>
            {
                

                // Définition des colonnes (ConstantColumn est très bien ici)
                table.ColumnsDefinition(column =>
                {
                    column.ConstantColumn(42);  // Réf Examen
                    column.RelativeColumn(3);   // Description
                    column.ConstantColumn(42);  // Qté
                    column.ConstantColumn(85);  // Px Unitaire (4ème colonne)
                    column.ConstantColumn(95); // Montant HT (5ème colonne)
                });

                // 1. Définition des En-têtes (Header)
                table.Header(header =>
                {
                    // Appliquer BorderBottom(1) sur tout l'header pour la séparation horizontale
                    header.Cell().BorderBottom(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Réf.").Bold().Style(TextStyle.Default.FontSize(10));
                    header.Cell().BorderBottom(1).Background(Colors.Grey.Lighten3).AlignLeft().Padding(5).Text("Désignation").Bold().Style(TextStyle.Default.FontSize(10));
                    header.Cell().BorderBottom(1).Background(Colors.Grey.Lighten3).AlignRight().Padding(5).Text("Qté").Bold().Style(TextStyle.Default.FontSize(10));

                    // 💡 Px Unitaire : Bordure Droite (Verticale)
                    header.Cell().BorderBottom(1).BorderRight(1).BorderColor(Colors.Black)
                        .Background(Colors.Grey.Lighten3).AlignRight().Padding(5).Text("Px Unitaire").Bold().Style(TextStyle.Default.FontSize(10));

                    header.Cell().BorderBottom(1).Background(Colors.Grey.Lighten3).AlignRight().Padding(5).Text("Montant HT").Bold().Style(TextStyle.Default.FontSize(10));
                });

                // 2. Corps du tableau (Lignes d'examens)
                foreach (var line in lines)
                {
                    AvailableContentSize--;
                    var examen = line.Examen;
                     decimal totalHtLigne = examen!.Price * line.Qte;

                    // Lignes de données
                    table.Cell().Padding(5).Text(examen.Reference.ToString()).FontSize(9);
                    table.Cell().Padding(5).Text(examen.ExamenName).FontSize(11).SemiBold();
                    table.Cell().AlignRight().Padding(5).Text(line.Qte.ToString()).FontSize(9);

                    // 💡 Px Unitaire : Bordure Droite (Verticale)
                    table.Cell().BorderRight(1).BorderColor(Colors.Black)
                        .Padding(5).AlignRight().Text($"{examen.Price:C}").FontSize(10);

                    table.Cell().Padding(5).AlignRight().Text($"{totalHtLigne:C}").FontSize(10);
                    
                }

                // 💡 Ajouter une ligne vide pour étendre la bordure (si le tableau est court)
                // Vous devez boucler sur un nombre prédéfini pour assurer une hauteur minimale,
                // OU appliquer un style à la dernière cellule pour remplir l'espace.

                // Ici, nous ajoutons une seule "cellule" qui couvre 4 colonnes, laissant 
                while(AvailableContentSize > 0)
                {
                    AvailableContentSize--;
                    table.Cell().Padding(5).Text("").FontSize(10); // Ligne vide pour la bordure inférieure;
                    table.Cell().Padding(5).Text("").FontSize(10);
                    table.Cell().Padding(5).Text("").FontSize(10);
                    table.Cell().BorderRight(1).BorderColor(Colors.Black)
                        .Padding(5).AlignRight().Text("").FontSize(10);

                    table.Cell().Padding(5).AlignRight().Text("").FontSize(10);
                }

                // Cellules de la première ligne du Footer (Ex: Total HT)
                table.Cell().ColumnSpan(3).BorderTop(1).Padding(2).Text("Total HT").Bold().FontSize(11).FontColor(Colors.Blue.Darken4);

                // 💡 Cellule sous Px Unitaire : Garder la bordure droite
                table.Cell().BorderRight(1).BorderTop(1).BorderColor(Colors.Black).Text("").Bold();

                // Cellule sous Montant HT : Afficher le total
                table.Cell().BorderTop(1).AlignRight().Padding(2).Text($"{totalHT:C}").Bold().FontSize(12).FontColor(Colors.Blue.Darken4);

                // CSS 
                table.Cell().ColumnSpan(3).BorderTop(1).Padding(1).Text($"CSS {_facture.Css:P0}").SemiBold().FontSize(9).FontColor(Colors.Black);
                table.Cell().BorderRight(1).BorderTop(1).BorderColor(Colors.Black).Text("");
                table.Cell().BorderTop(1).AlignRight().Padding(1).Text($"{CSS:C}").SemiBold().FontSize(9).FontColor(Colors.Black);
                // TPS
                table.Cell().ColumnSpan(3).Padding(1).Text($"TPS {_facture.TPS:P0}").SemiBold().FontSize(9).FontColor(Colors.Black);
                table.Cell().BorderRight(1).BorderColor(Colors.Black).Text("");
                table.Cell().BorderTop(1).AlignRight().Padding(1).Text($"{TPS:C}").SemiBold().FontSize(9).FontColor(Colors.Black);
                // Total TTC 
                table.Cell().ColumnSpan(3).BorderTop(1).Padding(2).Text("Total TTC").Bold().FontSize(11).FontColor(Colors.Blue.Darken4);
                table.Cell().BorderRight(1).BorderTop(1).BorderColor(Colors.Black).Text("").Bold();
                table.Cell().BorderTop(1).AlignRight().Padding(2).Text($"{totalTTC:C}").Bold().FontSize(12).FontColor(Colors.Blue.Darken4);

                // Remise
                if(remiseFlat > 0)
                    table.Cell().ColumnSpan(3).PaddingRight(9).Padding(1).Text($"Remise ({_facture.DiscountFlat:N0})").FontSize(9);
                else if(remisePercent > 0)
                    table.Cell().ColumnSpan(3).PaddingRight(9).Padding(1).Text($"Remise ({_facture.DiscountPercent:P0})").FontSize(9);
                else
                    table.Cell().ColumnSpan(3).PaddingRight(9).Padding(1).Text($"Remise (0%)").FontSize(9);

                table.Cell().BorderRight(1).BorderColor(Colors.Black).Text("").Bold();
                table.Cell().AlignRight().Padding(1).Text($"{totalWithRemise:C}").FontSize(9);

                // Net à payer
                table.Cell().ColumnSpan(3).PaddingRight(-12).BorderLeft(1).Padding(1).Text("Net à payer").Bold().FontSize(12).FontColor(Colors.Green.Darken4);
                table.Cell().BorderRight(1).BorderColor(Colors.Black).Text("").Bold();
                table.Cell().AlignRight().Padding(1).Text($"{netAPayer:C}").Bold().FontSize(12).FontColor(Colors.Green.Darken4);

                // Avance
                table.Cell().ColumnSpan(3).PaddingRight(9).Padding(1).Text("Avance").FontSize(9);
                table.Cell().BorderRight(1).BorderColor(Colors.Black).Text("").Bold();
                table.Cell().AlignRight().Padding(1).Text($"{amountPaid:C}").FontSize(9);

                if (amountPaid > 0)
                {
                    // Reste à payer
                    table.Cell().ColumnSpan(3).BorderTop(1).PaddingRight(-17).Padding(2).Text("Reste à payer").FontSize(12).Bold();
                    table.Cell().BorderTop(1).BorderRight(1).BorderColor(Colors.Black).Text("");
                    table.Cell().BorderTop(1).AlignRight().Padding(2).Text($"{amountDue:C}").FontSize(10);
                }
                else
                {
                    // Reste à payer
                    table.Cell().ColumnSpan(3).BorderTop(1).PaddingRight(-17).Padding(2).Text("Reste à payer").FontSize(12);
                    table.Cell().BorderTop(1).BorderRight(1).BorderColor(Colors.Black).Text("");
                    table.Cell().BorderTop(1).AlignRight().Padding(2).Text($"0 FCFA").FontSize(10);
                }
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