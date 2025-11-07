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
    public class Examen : ObservableObject
    {
        private int _examenId;
        private string _reference = string.Empty;
        private string _examenName = string.Empty;
        private decimal _price;
        private DateTime _createdAt = DateTime.Now;

        public int ExamenId
        {
            get => _examenId;
            set => SetProperty(ref _examenId, value);
        }
        [Column(TypeName = "nvarchar(8)")]
        public string Reference
        {
            get => _reference;
            set
            {
                if (InputValidator.IsValidReferenceString(value))
                    SetProperty(ref _reference, value);
            }
        }

        public string? ExamenName
        {
            get => _examenName; 
            set => SetProperty(ref _examenName, InputValidator.ToLowerString(value) ?? string.Empty);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }

        public ICollection<Facture> Factures { get; set; } = new List<Facture>();
        public ICollection<FactureExamen> FacturesExamens { get; set; } = new List<FactureExamen>();

        public override string ToString() => ExamenName;
    }
}
