using invoice.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        [Column(TypeName = "nvarchar(8)")]
        public int Reference { get; set; }
        public InvoiceType Type { get; set; } = InvoiceType.Autre; // Consultation, Examen, Autre
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Total_amount { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal Tva { get; set; }
        public decimal Css { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal Assurance_coverage_percent { get; set; } // prise en charge
        [Column(TypeName = "decimal(3,2)")]
        public decimal Patient_responsibility { get; set; } // part patient
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount_paid { get; set; } //  accompte
        [Column(TypeName = "decimal(3,2)")]
        public decimal Discount_percent { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount_flat { get; set; } = 0;
        public StatusType? Status { get; set; } = StatusType.Non_payer; // En attente, Payée, Annulée
        public string? Payment_method { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        public required User User { get; set; }

        public int PatientId { get; set; }
        public required Patient Patient { get; set; }

        public ICollection<Examen> Examens { get; set; } = [];
        public ICollection<FactureExamen> FacturseExamens { get; set; } = [];

        public ICollection<Consultation> Consultations { get; set; } = [];
        public ICollection<FactureConsultation> FacturesConsultations { get; set; } = [];

        public FactureAvoir? FactureAvoir { get; set; }
    }
}
