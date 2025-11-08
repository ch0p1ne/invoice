using CommunityToolkit.Mvvm.ComponentModel;
using invoice.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class PrixHomologue : ObservableObject
    {
        
        private int _elementId;
        private string _reference = string.Empty;
        private string _elementName = string.Empty;
        private decimal _price;
        private int _categorieId;
        private DateTime _createdAt = DateTime.Now;

        [Key]
        public int ElementId
        {
            get => _elementId;
            set => SetProperty(ref _elementId, value);
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

        [Column(TypeName = "nvarchar(99)")]
        public string? ElementName
        {
            get => _elementName;
            set => SetProperty(ref _elementName, InputValidator.ToLowerString(value) ?? string.Empty);
        }

        public decimal Price
        {
            get => _price; set => SetProperty(ref _price, InputValidator.ValidPriceString(value));
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }

        public int CategorieId
        {
            get => _categorieId;
            set => SetProperty(ref _categorieId, value);
        }

        public Categorie Categorie { get; set; } = new();

        public override string ToString() => ElementName ?? string.Empty;
    }
}
