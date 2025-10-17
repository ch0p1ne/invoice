using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Column(TypeName = "nvarchar(75)")]
        public string Account_name { get; set; } = string.Empty;
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        [MaxLength(255)]
        public string Salt { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(100)")]
        public string? Email { get; set; }
        [Column(TypeName = "nvarchar(15)")]
        public string Phone_number_one { get; set; } = string.Empty;
        public DateTime Created_at { get; set; } = DateTime.Now;

        public ICollection<Role> Roles { get; set; } = [];
        public ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<Facture>? Factures { get; set; }

    }
}
