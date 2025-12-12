using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace invoice.Models
{
    public class Role : ObservableObject
    {
        private string _roleName = String.Empty;
        private string _roleDescription = String.Empty;
        public int RoleId { get; set; }

        [MaxLength(75)]
        public string Role_name
        { 
            get => _roleName; 
            set => SetProperty(ref _roleName, value);
        }
        [MaxLength(255)]
        public string Role_description
        {
            get => _roleDescription;
            set => SetProperty(ref _roleDescription, value);
        }
        public DateTime Created_at { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();

        // Collection de jonction explicite
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
