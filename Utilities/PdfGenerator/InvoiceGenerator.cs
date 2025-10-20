using QuestPDF.Infrastructure;
using System;
using invoice.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;


namespace invoice.Utilities.PdfGenerator
{
    public class InvoiceGenerator : IDocument
    {

        public Facture Facture { get;}

        public InvoiceGenerator(Facture facture)
        {
            Facture = facture;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(25);


                    page.Header().Height(100).Background(Colors.Grey.Lighten2);
                    page.Content().Background(Colors.Grey.Lighten3);
                    page.Footer().Height(75).Background(Colors.Grey.Lighten2);
                });
        }
    }
}
