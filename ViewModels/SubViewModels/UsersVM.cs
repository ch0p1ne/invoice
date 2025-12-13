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
        private Role? _selectedRole;
        private User? _selecdUser;
        private bool _IsOperationDone = false;

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
        public string UserName
        {
            get => _userName;
            set
            {
                SetProperty(ref _userName, InputValidator.ToUpperString(value) ?? value);
                CreateUserCommand.NotifyCanExecuteChanged();
                EditUserCommand.NotifyCanExecuteChanged();
            }
        }
        public string FullName
        {
            get => _fullName;
            set
            {
                SetProperty(ref _fullName, value);
                CreateUserCommand.NotifyCanExecuteChanged();
                EditUserCommand.NotifyCanExecuteChanged();
            }

        }

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                CreateUserCommand.NotifyCanExecuteChanged();
                EditUserCommand.NotifyCanExecuteChanged();
            }
        }
        public string Phone
        {
            get => _phone;
            set
            {
                SetProperty(ref _phone, InputValidator.FormatAndValidateInput(value) ?? value);
                CreateUserCommand.NotifyCanExecuteChanged();
                EditUserCommand.NotifyCanExecuteChanged();
            }

        }
        public SecureString PasswordSecureString { get => _passwordSecureString; set => SetProperty(ref _passwordSecureString, value); }
        public User? SelectedUser
        {
            get => _selecdUser;
            set
            {
                SetProperty(ref _selecdUser, value);
                DeleteUserCommand.NotifyCanExecuteChanged();
                LoadSelectedUserDataCommand.NotifyCanExecuteChanged();
            }
        }

        public Role? SelectedRole
        {
            get => _selectedRole;
            set
            {
                SetProperty(ref _selectedRole, value);
                CreateUserCommand.NotifyCanExecuteChanged();
                EditUserCommand.NotifyCanExecuteChanged();
            }
        }
        public bool IsOperationDone { get => _IsOperationDone; set => SetProperty(ref _IsOperationDone, value); }


        // Constructor
        public UsersVM()
        {
            _ = LoadUsersAsync();
        }

        // Commands
        [RelayCommand]
        public async Task LoadUsersAsync()
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
        [RelayCommand(CanExecute = nameof(CanCreateUser))]
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
                ClearInputFields();
                await LoadUsersAsync();
                IsOperationDone = true;
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
        [RelayCommand(CanExecute = nameof(CanDeleteUser))]
        public async Task DeleteUserAsync()
        {
            var messageBox = new ModelOpenner();
            if (messageBox.Show("Confirmer la suppression", "Êtes-vous sûr de vouloir supprimer cet utilisateur ?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                using var context = new ClimaDbContext();
                var userToDelete = await context.Users.FindAsync(SelectedUser!.UserId);
                if (userToDelete != null)
                {
                    context.Users.Remove(userToDelete);
                    await context.SaveChangesAsync();
                    Users.Remove(SelectedUser);
                    TotalUsersCount--;
                    if (userToDelete.IsActive)
                    {
                        TotalActifUsersCount--;
                    }
                    SelectedUser = null;
                    messageBox.Show("Succès", "Utilisateur supprimé avec succès", MessageBoxButton.OK);
                }
                else
                {
                    messageBox.Show("Réessayer", "Utilisateur non trouvé dans la base de données, les données sont peut etre compromis. Veuillez contacté l'administrateur.", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                messageBox.Show("Erreur", $"Une erreur est survenue lors de la suppression de l'utilisateur : {ex.Message}", MessageBoxButton.OK);
            }
        }
        [RelayCommand(CanExecute = nameof(CanLoadSelectedUserData))]
        public async Task LoadSelectedUserDataAsync()
        {
            FullName = SelectedUser.FullName;
            UserName = SelectedUser.Account_name;
            Email = SelectedUser.Email ?? string.Empty;
            Phone = SelectedUser.Phone_number_one;
            IsActivateAccountIsCheck = SelectedUser.IsActive;
            using var context = new ClimaDbContext();
            var role = await context.Roles.FindAsync(SelectedUser.RoleId);
            SelectedRole = role;

        }
        [RelayCommand(CanExecute = nameof(CanEditUser))]
        public async Task EditUserAsync()
        {
            var messageBox = new ModelOpenner();
            try
            {
                using var context = new ClimaDbContext();
                var userToEdit = await context.Users.FindAsync(SelectedUser!.UserId);
                if (userToEdit != null)
                {
                    string plainpwd = SecureStringHelper.ToPlainString(PasswordSecureString!);
                    string salt = PasswordHasher.GenerateSalt();
                    string hash = PasswordHasher.HashPassword(plainpwd, salt);

                    userToEdit.FullName = FullName;
                    userToEdit.Account_name = UserName;
                    userToEdit.Email = Email;
                    userToEdit.Phone_number_one = Phone;
                    userToEdit.IsActive = IsActivateAccountIsCheck;
                    userToEdit.PasswordHash = hash;
                    userToEdit.Salt = salt;
                    userToEdit.Updated_at = DateTime.Now;

                    if (SelectedRole != null)
                    {
                        userToEdit.RoleId = SelectedRole.RoleId;
                    }
                    if (userToEdit.IsActive)
                    {
                        TotalActifUsersCount++;
                    }
                    else
                    {
                        TotalActifUsersCount--;
                    }

                    ClearInputFields();
                    context.Users.Update(userToEdit);
                    await context.SaveChangesAsync();
                    await LoadUsersAsync();
                    IsOperationDone = true;
                    messageBox.Show("Succès", "Utilisateur modifié avec succès", MessageBoxButton.OK);
                }
                else
                {
                    messageBox.Show("Réessayer", "Utilisateur non trouvé dans la base de données, les données sont peut etre compromis. Veuillez contacté l'administrateur.", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                messageBox.Show("Erreur", $"Une erreur est survenue lors de la modification de l'utilisateur : {ex.Message}", MessageBoxButton.OK);
            }
        }
        [RelayCommand]
        public void ResetOperationFlag()
        {
            IsOperationDone = false;
        }
        [RelayCommand]
        public void ClearInputFields()
        {
            UserName = string.Empty;
            FullName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            PasswordSecureString = new SecureString();
            SelectedRole = null;
            IsActivateAccountIsCheck = false;
        }

        // CanExecute Methods
        public bool CanDeleteUser()
        {
            return SelectedUser != null;
        }
        public bool CanLoadSelectedUserData()
        {
            return SelectedUser != null;
        }
        public bool CanEditUser()
        {
            return (FullName != string.Empty && FullName.Length >= 3) &&
                   (UserName != string.Empty && UserName.Length >= 3) &&
                   (Phone != string.Empty && Phone.Length >= 8) &&
                   SelectedRole != null;

        }
        public bool CanCreateUser()
        {
            return (FullName != string.Empty && FullName.Length >= 3) &&
                   (UserName != string.Empty && UserName.Length >= 3) &&
                   (Phone != string.Empty && Phone.Length >= 8) &&
                   SelectedRole != null;
        }
    }
}
