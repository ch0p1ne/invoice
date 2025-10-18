using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class Examen : ObservableObject
    {
        private int _examenId;
        private int _reference;
        private string? _examenName;
        private decimal _price;
        private DateTime _createdAt = DateTime.Now;

        public int ExamenId
        {
            get => _examenId;
            set => SetProperty(ref _examenId, value);
        }

        public int Reference
        {
            get => _reference;
            set => SetProperty(ref _reference, value);
        }

        public string? ExamenName
        {
            get => _examenName;
            set => SetProperty(ref _examenName, value);
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
    }
}
