using CommunityToolkit.Mvvm.Input;
using invoice.Context;
using invoice.Models;
using invoice.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
            _ = LoadGrantedPermissionAsync();
            _ = LoadPermissionAsync();
        }
        
        public ObservableCollection<Permission> AvailablePermissions { get; set; } = new ObservableCollection<Permission>();
        public ObservableCollection<Permission> AvailableTmpPermissions { get; set; } = new ObservableCollection<Permission>();
        public ObservableCollection<Permission> GrantedPermissions { get; set; } = new ObservableCollection<Permission>();
        private Permission _selectedPermission;
        public Permission SelectedPermission
        {
            get => _selectedPermission;
            set
            {
                SetProperty(ref _selectedPermission, value);
                GrantPermissionCommand.NotifyCanExecuteChanged();
            }
        }
        private Permission _selectedGrantPermission;
        public Permission SelectedGrantPermission
        {
            get => _selectedGrantPermission;
            set
            {
                SetProperty(ref _selectedGrantPermission, value);
                RemovePermissionCommand.NotifyCanExecuteChanged();
            }
        }
        private Role _selectedRole;
        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                SetProperty(ref _selectedRole, value);
                RefreshPermissionList();
                _ = LoadGrantedPermissionAsync();
                GrantPermissionCommand.NotifyCanExecuteChanged();
                RemovePermissionCommand.NotifyCanExecuteChanged();
            }
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
        public async Task LoadPermissionAsync()
        {
            using var context = new ClimaDbContext();
            try
                {
                var permissions = await context.Permissions.ToListAsync();
                AvailablePermissions.Clear();
                foreach (var p in permissions)
                {
                    AvailablePermissions.Add(p);
                    AvailableTmpPermissions.Add(p);
                }
            }
            catch (Exception ex)
            {
                var messageBox = new ModelOpenner();
                messageBox.Show("Erreur", $"Une erreur est survenue lors du chargement des permissions : {ex.Message}", MessageBoxButton.OK);
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
        [RelayCommand(CanExecute = nameof(CanGrantPermission))]
        public async Task GrantPermissionAsync()
        {
            GrantedPermissions.Add(SelectedPermission);
            AvailablePermissions.Remove(SelectedPermission);
        }
        [RelayCommand(CanExecute = nameof(CanRemovePermission))]
        public async Task RemovePermissionAsync()
        {
            AvailablePermissions.Add(SelectedGrantPermission);
            GrantedPermissions.Remove(SelectedGrantPermission);
        }
        [RelayCommand]
        public async Task CreateRolePermissionsEntryAsync()
        {
            using var context = new ClimaDbContext();
            var messageBox = new ModelOpenner();

            if (SelectedRole == null)
            {
                messageBox.Show("Avertissement", "Veuillez sélectionner un rôle avant d'enregistrer les permissions.", MessageBoxButton.OK);
                return;
            }

            if (GrantedPermissions == null || GrantedPermissions.Count == 0)
            {
                messageBox.Show("Avertissement", "Aucune permission accordée à enregistrer pour ce rôle. Un permission minimum est requise pour appliquer les changement", MessageBoxButton.OK);
                return;
            }

            try
            {
                // Charger le rôle avec ses permissions actuelles
                var role = await context.Roles
                    .Include(r => r.RolePermissions)
                    .FirstOrDefaultAsync(r => r.RoleId == SelectedRole.RoleId);

                if (role == null)
                {
                    messageBox.Show("Erreur", "Le rôle sélectionné est introuvable dans la base de données.", MessageBoxButton.OK);
                    return;
                }

                // Synchroniser : supprimer les liaisons existantes puis ajouter celles de GrantedPermissions
                role.RolePermissions.Clear();

                foreach (var gp in GrantedPermissions)
                {
                    // Récupérer l'entité Permission traquée par le contexte
                    var permission = await context.Permissions.FindAsync(gp.PermissionId);
                    if (permission != null && !role.RolePermissions.Any(p => p.PermissionId == permission.PermissionId))
                    {
                        var rolePermission = new RolePermission
                        {
                            RoleId = role.RoleId,
                            PermissionId = permission.PermissionId
                        };
                        role.RolePermissions.Add(rolePermission);
                    }
                }

                await context.SaveChangesAsync();

                messageBox.Show("Succès", "Les permissions du rôle ont été enregistrées avec succès.", MessageBoxButton.OK);

            }
            catch (Exception ex)
            {
                messageBox.Show("Erreur", $"Une erreur est survenue lors de l'enregistrement des permissions : {ex.Message}", MessageBoxButton.OK);
            }
        }

        // Methods for granting and revoking permissions can be added here
        public async Task LoadGrantedPermissionAsync()
        {
            using var context = new ClimaDbContext();

            try
            {
                GrantedPermissions.Clear();
                if (SelectedRole != null)
                {
                    var roleWithPermissions = await context.Roles
                        .Include(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                        .FirstOrDefaultAsync(r => r.RoleId == SelectedRole.RoleId);
                    if (roleWithPermissions != null)
                    {
                        foreach (var rp in roleWithPermissions.RolePermissions)
                        {
                            GrantedPermissions.Add(rp.Permission);
                            var permitionAlreadyGrant = AvailablePermissions.FirstOrDefault(p => p.Permission_name == rp.Permission.Permission_name);
                            if (permitionAlreadyGrant != null)
                                AvailablePermissions.Remove(permitionAlreadyGrant);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var messageBox = new ModelOpenner();
                messageBox.Show("Erreur", $"Une erreur est survenue lors du chargement des permissions accordées : {ex.Message}", MessageBoxButton.OK);
                return;
            }
        }
        public void RefreshPermissionList()
        {
            foreach (var p in AvailableTmpPermissions)
            {
                if (AvailablePermissions.Contains(p))
                    continue;
                AvailablePermissions.Add(p);
            }
        }

        // CanExecute methods

        private bool CanGrantPermission()
        {
            return SelectedPermission != null && SelectedRole != null;
        }
        private bool CanRemovePermission()
        {
            return SelectedGrantPermission != null && SelectedRole != null;
        }

        }
}
