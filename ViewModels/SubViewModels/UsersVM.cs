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
        private int _totalActifUsersCount;

        // Property
        public ObservableCollection<User> Users { get; set; } = new();
        public int TotalUsersCount
        {
            get => _totalUsersCount;
            set => SetProperty(ref _totalUsersCount, value);
        }
        public int TotalActifUsersCount 
        { 
            get => _totalActifUsersCount;
            set => SetProperty(ref _totalActifUsersCount, value);
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
                TotalActifUsersCount = users.Count(u => u.IsActive);
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
