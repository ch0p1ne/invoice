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
    public class Patient : ObservableObject
    {
        private string _lastName = string.Empty;
        private string _firstName = string.Empty;
        private string _phoneNumber = string.Empty;
        private string? _assuranceNumber = string.Empty;
        public int PatientId { get; set; }
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, InputValidator.CapitalizeEachWord(value) ?? value); 
        }
        public string LastName
        { 
            get => _lastName;
            set => SetProperty(ref _lastName, InputValidator.ToUpperString(value) ?? value);
        }
        public DateTime? DateOfBirth { get; set; } = DateTime.MinValue;

        [Column(TypeName = "nvarchar(13)")]
        public string? PhoneNumber
        { 
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, InputValidator.FormatAndValidateInput(value) ?? value);
        }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? AssuranceNumber
        { 
            get => _assuranceNumber;
            set => SetProperty(ref _assuranceNumber, value);
        }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? AssuranceId { get; set; }
        public Assurance? Assurance { get; set; }

        public ICollection<Facture> Factures { get; set; } = [];

        public override string ToString() => LastName+ " " + FirstName;
    }
}
