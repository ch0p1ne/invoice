using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class FactureConsultation
    {
        public int FactureId { get; set; }
        public int ConsultationId { get; set; }
        [Range(0, 100)]
        public int Qte { get; set; } = 1;

        public int MedecinId { get; set; }
        public required Medecin Medecin { get; set; }
    }
}
