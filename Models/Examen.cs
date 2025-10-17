using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class Examen
    {
        public int ExamenId { get; set; }
        public int Reference { get; set; }
        public string? ExamenName { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Facture> Factures { get; set; } = [];
        public ICollection<FactureExamen> FacturesExamens { get; set; } = [];
    }
}
