using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class Consultation
    {
        public int ConsultationId { get; set; }
        public int Reference { get; set; }
        public string? ConsultationName { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Facture> Factures { get; set; } = [];
        public ICollection<FactureConsultation> FacturesConsultations { get; set; } = [];
    }
}
