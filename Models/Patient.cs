using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; } = DateTime.MinValue;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? AssuranceNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? AssuranceId { get; set; }
        public Assurance? Assurance { get; set; }

        public ICollection<Facture> Factures { get; set; } = [];

        public override string ToString() => LastName + FirstName;
    }
}
