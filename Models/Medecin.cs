using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class Medecin
    {

        public int MedecinId { get; set; }
        [Column(TypeName=("nvarchar(50)"))]
        public string MedecinFirstName { get; set; } = string.Empty;
        [Column(TypeName = ("nvarchar(50)"))]
        public string MedecinLastName { get; set; } = string.Empty;
        [Column(TypeName = ("nvarchar(50)"))]
        public string Speciality { get; set; } = string.Empty;
        [Column(TypeName = ("nvarchar(75)"))]
        public string? Address { get; set; }
        [Column(TypeName = ("nvarchar(15)"))]
        public string? PhoneNumberOne { get; set; }
        [Column(TypeName = ("nvarchar(15)"))]
        public string? PhoneNumberTwo { get; set; }
        public DateTime StartWork { get; set; } = DateTime.Now;
        public DateTime? EndWork { get; set; } = null;
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<FactureConsultation>? FacturesConsultations { get; set; }

        public override string ToString()
        {
            return $"{MedecinFirstName} {MedecinLastName} - {Speciality}";
        }
    }
}
