using CommunityToolkit.Mvvm.Input;
using invoice.Context;
using invoice.Models;
using invoice.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace invoice.ViewModels.SubViewModels
{
    public partial class UsersVM : VMBase
    {
        // Fields
        private int _totalUsersCount;
        private int _totalActifUsersCount;
        private bool _isActivateAccountIsCheck = false;
        private string _userName = string.Empty;
        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private SecureString _passwordSecureString;
        private Role _selectedRole;

        // Property
        public bool IsActivateAccountIsCheck
        {
            get => _isActivateAccountIsCheck;
            set => SetProperty(ref _isActivateAccountIsCheck, value);
        }
        public ObservableCollection<Role> AvailableRoles { get; set; } = new ObservableCollection<Role>();
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
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
        public string UserName { get => _userName; set => SetProperty(ref _userName, InputValidator.ToUpperString(value) ?? value); }
        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Phone { get => _phone; set => SetProperty(ref _phone, InputValidator.FormatAndValidateInput(value) ?? value); }
        public SecureString PasswordSecureString { get => _passwordSecureString; set => SetProperty(ref _passwordSecureString, value); }
        public Role SelectedRole { get => _selectedRole; set => SetProperty(ref _selectedRole, value); }


        // Constructor
        public UsersVM()
        {
            _ = LoadUsersAsync();
        }


        // Methods
        private async Task LoadUsersAsync()
        {
            var messageBox = new ModelOpenner();
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
                var Roles = await context.Roles.ToListAsync();
                
                AvailableRoles.Clear();
                foreach (var role in Roles)
                {
                    AvailableRoles.Add(role);
                }
            }
            catch (Exception ex)
            {
                messageBox.Show("Erreur", $"Une erreur est survenue lors du chargement des utilisateurs: {ex.Message}", MessageBoxButton.OK);
            }
        }

        // Commands
        [RelayCommand]
        public async Task CreateUserAsync()
        {
            var messageBox = new ModelOpenner();

            if (string.IsNullOrWhiteSpace(UserName) ||
               string.IsNullOrWhiteSpace(FullName) ||
               string.IsNullOrWhiteSpace(Phone) ||
               string.IsNullOrWhiteSpace(SecureStringHelper.ToPlainString(PasswordSecureString)) ||
               SelectedRole == null)
            {
                messageBox.Show("Réessayer", "Entrez tous les champs requis", MessageBoxButton.OK);
                return;
            }
            try
            {
                using var context = new ClimaDbContext();

                bool exits = await context.Users.AnyAsync(u => u.Account_name == UserName);
                if (exits)
                {
                    messageBox.Show("Réessayer", $"L'utilisateur avec l'identifiant '{UserName}' existe deja", MessageBoxButton.OK);
                    return;
                }

                string plainpwd = SecureStringHelper.ToPlainString(PasswordSecureString!);
                string salt = PasswordHasher.GenerateSalt();
                string hash = PasswordHasher.HashPassword(plainpwd, salt);

                // Dans User : public int RoleId { get; set; }
                var newUser = new User
                {
                    Account_name = UserName,
                    FullName = FullName,
                    Email = Email,
                    Phone_number_one = Phone,
                    PasswordHash = hash,
                    Salt = salt,
                    RoleId = SelectedRole.RoleId,
                    IsActive = IsActivateAccountIsCheck
                };
                context.Users.Add(newUser);
                await context.SaveChangesAsync();
                Users.Add(newUser);
                TotalUsersCount++;
                if (newUser.IsActive)
                {
                    TotalActifUsersCount++;
                }
                // Clear input fields
                UserName = string.Empty;
                FullName = string.Empty;
                Email = string.Empty;
                Phone = string.Empty;
                SelectedRole = null;
                IsActivateAccountIsCheck = false;
                await LoadUsersAsync();
                messageBox.Show("Succès", "Utilisateur créé avec succès", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                messageBox.Show("Erreur", $"Une erreur est survenue lors de la création de l'utilisateur : {ex.Message}", MessageBoxButton.OK);
                if (ex.InnerException != null)
                {
                    messageBox.Show("Erreur", $"Erreur Interne : {ex.InnerException.Message}", MessageBoxButton.OK);
                }
            }
        }
    }
}
