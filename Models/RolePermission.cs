using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.Models
{
    public class RolePermission
    {
        // Composite key: RoleId + PermissionId
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public int PermissionId { get; set; }
        public Permission? Permission { get; set; }

        // Exemple de méta-donnée sur la relation (optionnel)
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    }
}
