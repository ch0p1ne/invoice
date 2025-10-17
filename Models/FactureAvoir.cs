using invoice.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class FactureAvoir
    {
        public int FactureAvoirId { get; set; }
        public string? Raison { get; set; }
        public int Reference { get; set; }
        public InvoiceType Type { get; set; } // Consultation, Examen, Autre
        public decimal TotalAmount { get; set; }
        public decimal Tva { get; set; }
        public decimal Css { get; set; }
        public decimal AssuranceCoveragePercent { get; set; } // prise en charge
        public decimal PatientResponsibility { get; set; } // part patient
        public decimal AmountPaid { get; set; } //  accompte
        public decimal Discount_percent { get; set; } = 0;
        public decimal Discount_flat { get; set; } = 0;
        public StatusType Status { get; set; } = StatusType.Non_payer; // En attente, Payée, Annulée
        public string? PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int FactureId { get; set; }
        public required Facture Facture { get; set; }
    }
}
