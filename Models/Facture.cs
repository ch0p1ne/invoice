using invoice.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class Facture
    {
        public int FactureId { get; set; }
        [Column(TypeName = "nvarchar(15)")]
        public string Reference { get; set; } = string.Empty;
        public InvoiceType Type { get; set; } = InvoiceType.Autre; // Consultation, Examen, Autre
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmountHT { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmountTTC { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal Tva { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? InsuranceCoveragePercent { get; set; } // prise en charge
        [Column(TypeName = "decimal(3,2)")]
        public decimal PatientPercent { get; set; } // part patient
        [Column(TypeName = "decimal(10,2)")]
        public decimal? AmountPaid { get; set; } //  accompte
        [Column(TypeName = "decimal(3,2)")]
        public decimal? DiscountPercent { get; set; } = 0;
        public double? DiscountFlat { get; set; } = 0;
        [Column(TypeName = "decimal(3,2)")]
        public decimal Css { get; set; } = 0.00m;
        [Column(TypeName = "decimal(3,2)")]
        public decimal TPS { get; set; } = 0.00m;
        public StatusType? Status { get; set; } = StatusType.Non_payer; // En attente, Payée, Annulée
        public string? PaymentMethod { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;

        public required int UserId { get; set; }
        public User? User { get; set; }

        public required int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public ICollection<Examen> Examens { get; set; } = [];
        public ICollection<FactureExamen> FacturesExamens { get; set; } = [];

        public ICollection<Consultation> Consultations { get; set; } = [];
        public ICollection<FactureConsultation> FacturesConsultations { get; set; } = [];

        public FactureAvoir? FactureAvoir { get; set; }

    }
}
