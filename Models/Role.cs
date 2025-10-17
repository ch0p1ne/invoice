using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        
        public string Role_name { get; set; } = string.Empty;
        public int Create_patient { get; set; } = 0;
        public int Create_fac { get; set; } = 0;
        public int Manage_fac { get; set; } = 0;
        public int Manage_user { get; set; } = 0;
        public int Manage_med { get; set; } = 0;
        public int Manage_exam { get; set; } = 0;
        public int Manage_consul { get; set; } = 0;
        public DateTime Created_at { get; set; }

        public ICollection<User> Users { get; set; } = [];
        public ICollection<UserRole> UserRoles { get; set; } = [];

    }
}
