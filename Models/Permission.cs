using invoice.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Permission
    {
        public int PermissionId { get; set; }
        [MaxLength(75)]
        public string Permission_name { get; set; } = string.Empty;
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;
        public PermissionCategorie Categorie { get; set; }
        public DateTime Created_at { get; set; }

        public DateTime Updated_at { get; set; }

        // Collection de jonction explicite
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        public override string ToString() => Permission_name;
    }
}
