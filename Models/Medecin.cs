using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class Medecin
    {
        public int MedecinId { get; set; }
        public string? MedecinName { get; set; }
        public string? PhoneNumberOne { get; set; }
        public string? PhoneNumberTwo { get; set; }
        public DateTime StartWork { get; set; } = DateTime.Now;
        public DateTime? EndWork { get; set; } = null;
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<FactureConsultation>? FacturesConsultations { get; set; }
    }
}
