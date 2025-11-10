using CommunityToolkit.Mvvm.ComponentModel;
using invoice.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class Consultation : ObservableObject
    {
        // fields
        private string _reference = string.Empty;
        private string _consultationName = string.Empty;
        private decimal _price;

        public int ConsultationId { get; set; }
        [Column(TypeName = "nvarchar(8)")]
        public string Reference
        {
            get => _reference;
            set
            {
                if (InputValidator.IsValidReferenceString(value))
                    SetProperty(ref _reference, InputValidator.ToUpperString(value) ?? string.Empty);
            }
        }
        [Column(TypeName = "nvarchar(99)")]
        public string ConsultationName
        {
            get => _consultationName;
            set => SetProperty(ref _consultationName, InputValidator.ToUpperString(value) ?? string.Empty);
        }
        public decimal Price
        {
            get => _price; 
            set => SetProperty(ref _price, InputValidator.ValidPriceString(value));
        }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Facture> Factures { get; set; } = [];
        public ICollection<FactureConsultation> FacturesConsultations { get; set; } = [];
    }
}
