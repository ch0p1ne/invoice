using CommunityToolkit.Mvvm.Input;
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
using System.Windows;
using System.Windows.Data;

namespace invoice.ViewModels.SubViewModels
{
    public partial class RolesVM : VMBase
    {
        // Constructor
        public RolesVM()
        {
            _ = LoadRolesAsync();
        }

        private Role _selectedRole;
        public Role SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value);
        }
        public ObservableCollection<Role> RolesList { get; set; } = new ObservableCollection<Role>();

        private string _roleName = string.Empty;
        public string RoleName
        {
            get => _roleName;
            set => SetProperty(ref _roleName, InputValidator.CapitalizeEachWord(value) ?? value);
        }
        private string _roleDescription = string.Empty;
        public string RoleDescription
        {
            get => _roleDescription;
            set => SetProperty(ref _roleDescription, value);
        }

        private int _totalRolesCount;
        public int TotalRolesCount
        {
            get => _totalRolesCount;
            set => SetProperty(ref _totalRolesCount, value);
        }


        // Commands
        [RelayCommand]
        public async Task AddRoleAsync()
        {
            using var context = new ClimaDbContext();
            try
            {
                var messageBox = new ModelOpenner();
                if (string.IsNullOrWhiteSpace(RoleName))
                {
                    var messageBoxAdvertissement = messageBox.Show("Avertissement", "Le nom du rôle ne peut pas être vide.", MessageBoxButton.OK);
                    return;
                }
                var existingRole = context.Roles.FirstOrDefault(r => r.Role_name.ToLower() == RoleName.ToLower());

                if (existingRole != null)
                {
                    var messageBoxAdvertisement = messageBox.Show("Avertissement", "Un rôle avec ce nom existe déjà.", MessageBoxButton.OK);
                    return;
                }

                var newRole = new Role
                {
                    Role_name = RoleName,
                    Role_description = RoleDescription
                };
                context.Roles.Add(newRole);
                await context.SaveChangesAsync();

                // Mise à jour sur le thread UI
                var dispatcher = System.Windows.Application.Current?.Dispatcher;
                void AddAndReset()
                {
                    RoleName = string.Empty;
                    RoleDescription = string.Empty;
                    // ajoute dans la collection existante (référence inchangée)
                    RolesList.Add(newRole);
                    TotalRolesCount += 1;
                }

                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(AddAndReset);
                }
                else
                {
                    AddAndReset();
                }
                
                messageBox.Show("Succès", "Le rôle a été ajouté avec succès.", MessageBoxButton.OK);
                CollectionViewSource.GetDefaultView(RolesList).Refresh();
            }
            catch (Exception ex)
            {
                var messageBox = new ModelOpenner();
                messageBox.Show("Erreur", $"Une erreur est survenue lors de la création du rôle : {ex.Message}", MessageBoxButton.OK);
                return;
            }
        }

        [RelayCommand]
        public async Task LoadRolesAsync()
        {
             
            
            using var context = new ClimaDbContext();
            try
            {
                var messageBox = new ModelOpenner();
                var roles = await context.Roles.ToListAsync();

                var dispatcher = Application.Current?.Dispatcher;
                void UpdateCollection()
                {
                    TotalRolesCount = roles.Count;
                    // NE PAS remplacer la référence de RolesList : vider et ajouter
                    RolesList.Clear();
                    foreach (var r in roles)
                    {
                        RolesList.Add(r);
                    }
                }

                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(UpdateCollection);
                }
                else
                {
                    UpdateCollection();
                }
            }
            catch (Exception ex)
            {
                var messageBox = new ModelOpenner();
                messageBox.Show("Erreur", $"Une erreur est survenue lors du chargement des rôles : {ex.Message}", MessageBoxButton.OK);
                return;
            }
        }

        [RelayCommand]
        public async Task RefreshRolesAsync()
        {
            await LoadRolesAsync();
        }
        [RelayCommand]
        public async Task DeleteRoleAsync()
        {
            var messageBox = new ModelOpenner();
            if (SelectedRole == null)
            {
                messageBox.Show("Avertissement", "Veuillez sélectionner un rôle à supprimer.", MessageBoxButton.OK);
                return;
            }
            if (messageBox.Show("Confirmation", $"Êtes-vous sûr de vouloir supprimer le rôle '{SelectedRole.Role_name}' ?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            using var context = new ClimaDbContext();
            try
            {
                var roleToDelete = await context.Roles.FindAsync(SelectedRole.RoleId);
                if (roleToDelete != null)
                {
                    context.Roles.Remove(roleToDelete);
                    await context.SaveChangesAsync();
                    RolesList.Remove(SelectedRole);
                    TotalRolesCount -= 1;
                    SelectedRole = null;
                    messageBox.Show("Succès", "Le rôle a été supprimé avec succès.", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                messageBox.Show("Erreur", $"Une erreur est survenue lors de la suppression du rôle : {ex.Message}", MessageBoxButton.OK);
                return;
            }
        }
    }
}
