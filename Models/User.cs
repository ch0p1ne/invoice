using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace invoice.Models
{
    public class User : ObservableObject
    {
        private int _userId;
        private string _account_name = string.Empty;
        private string _fullName = string.Empty;
        private string _passwordHash = string.Empty;
        private string? _email;
        private bool _isActive = false;
        private string _phone_number_one = string.Empty;

        public int UserId { get => _userId; set => SetProperty(ref _userId, value); }
        [Column(TypeName = "nvarchar(50)")]
        public string Account_name { get => _account_name; set => SetProperty(ref _account_name, value); }
        [MaxLength(255)]
        public string PasswordHash { get => _passwordHash; set => SetProperty(ref _passwordHash, value); }
        [MaxLength(99)]
        public string FullName
        { 
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }
        [MaxLength(255)]
        public string Salt { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(50)")]
        public string? Email { get => _email; set => SetProperty(ref _email, value); }
        [Column(TypeName = "nvarchar(15)")]
        public string Phone_number_one { get => _phone_number_one; set => SetProperty(ref _phone_number_one, value); }
        [Column("is_active")]
        public bool IsActive
        { 
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
        public DateTime Created_at { get; set; } = DateTime.Now;

        public DateTime Updated_at { get; set; } = DateTime.Now;
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<Facture>? Factures { get; set; }


    }
}
