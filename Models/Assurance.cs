using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class Assurance
    {
        public int AssuranceId { get; set; }
        public string? Compagny { get; set; }
        public decimal CoveragePercent { get; set; } // pourcentage de prise en charge
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Patient>? Patients { get; set; }
    }
}
