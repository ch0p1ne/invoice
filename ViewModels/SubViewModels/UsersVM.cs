using invoice.Context;
using invoice.Models;
using invoice.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.ViewModels.SubViewModels
{
    public partial class UsersVM : VMBase
    {
        // Fields
        private int _totalUsersCount;

        // Property
        public ObservableCollection<User> Users { get; set; } = new();
        public int TotalUsersCount
        {
            get => _totalUsersCount;
            set => SetProperty(ref _totalUsersCount, value);
        }


        // Constructor
        public UsersVM()
        {
            LoadUsersAsync().ConfigureAwait(false);
        }


        // Methods
        private async Task LoadUsersAsync()
        {
            try
            {
                using var context = new ClimaDbContext();
                var users = await context.Users.ToListAsync();
                TotalUsersCount = users.Count;
                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occur while loading users: {ex.Message}");
            }
        }
    }
}
