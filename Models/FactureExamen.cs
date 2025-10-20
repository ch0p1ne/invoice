using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class FactureExamen
    {
        // Clé étrangère 1 (doit être une propriété)
        public int FactureId { get; set; }
        public Facture? Facture { get; set; }

        // Clé étrangère 2 (doit être une propriété)
        public int ExamenId { get; set; }
        public Examen? Examen { get; set; }

        [Range(0, 100)]
        public int Qte { get; set; }
    }
}
