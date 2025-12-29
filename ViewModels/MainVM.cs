using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using invoice.Context;
using invoice.Models;
using invoice.Services;
using invoice.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MenuItem = invoice.Utilities.MenuItem;

namespace invoice.ViewModels
{
    public partial class MainVM : VMBase
    {
        private bool _isCkecked = false;
        public bool IsChecked
        {
            get => _isCkecked;
            set
            {
                _isCkecked = value;
                OnPropertyChanged();
            }
        }
        private ISessionService _sessionService;
        private readonly INavigationService _navigationService;
        public ObservableCollection<MenuItem> MenuItems { get; }

        private readonly DateOnly _dateOnly = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly DateOnly => _dateOnly;
        public ICollection<Permission> UserPermissions { get; set; } = new List<Permission>();
        public ICollection<Permission> AvailablePermissions { get; set; } = new List<Permission>();



        public ISessionService SessionService => _sessionService;
        [ObservableProperty]
        private VMBase _currentViewModel;


        public MainVM(INavigationService navigationService, ISessionService sessionService)
        {
            _navigationService = navigationService;
            _sessionService = sessionService;
            MenuItems = new ObservableCollection<Utilities.MenuItem>()
            {

                new MenuItem("Nouvelle Facture", null, new CreateFactureVM(sessionService), "/Assets/icons/add.png"),
                new MenuItem("Vue des Factures", null, new FactureViewVM(), "/Assets/icons/list.png"),

                new MenuItem("Utilisateurs et Rôles", null, new UserAndRoleVM(), "/Assets/icons/users.png"),
                new MenuItem("Médecins", null, new MedecinVM(), "/Assets/icons/online-appointment.png"),
                     //new MenuItem("Assurances", null, new AssuranceVM(), "/Assets/icons/insurance.png"),

                new MenuItem("Consultation", null, new ConsultationVM(), "/Assets/icons/answer.png"),
                new MenuItem("Examen", null, new ExamenVM(), "/Assets/icons/exam.png"),
                // new MenuItem("Prix Homologue", null, new PrixHomologueVM(), "/Assets/icons/online-appointment.png"),
            };
        }

        public async Task InitializeAsync()
        {
            await LoadAvailablePermissions();
            await LoadUserPermissions();
            MenuItems[0].RequiredPermission = AvailablePermissions?.FirstOrDefault(ap => ap.Permission_name == "CREATE_INVOICES");
            MenuItems[1].RequiredPermission = AvailablePermissions?.FirstOrDefault(ap => ap.Permission_name == "INVOICES_VIEW");
            MenuItems[2].RequiredPermission = AvailablePermissions?.FirstOrDefault(ap => ap.Permission_name == "USERS_ROLES_VIEW");
            MenuItems[3].RequiredPermission = AvailablePermissions?.FirstOrDefault(ap => ap.Permission_name == "MEDECINS_VIEW");

            // Vérifier si l'utilisateur possède la permission requise.
            if (UserPermissions.Any(up => up.Permission_name == MenuItems[0]?.RequiredPermission?.Permission_name))
            {
                CurrentViewModel = new CreateFactureVM(_sessionService);
                IsChecked = true;
                return;
            }
            else
            {
                CurrentViewModel = new MissPermissionVM();
                IsChecked = true;
            }
        }

        [RelayCommand]
        public async Task Logout()
        {
            var messageBox = new ModelOpenner();

            if (messageBox.Show("Confirmation", "Voulez vous vraiment fermer la séssion ?", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
                return;
            try
            {
                using var context = new ClimaDbContext();
                _sessionService.User!.LastConnection = DateTime.Now;
                context.Users.Update(_sessionService.User);

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                messageBox.Show("Erreur survenue pendant la déconnexion", $"{ex.Message}", System.Windows.MessageBoxButton.OK);
            }
            _navigationService.NavigateTo<LoginVM>();
            _navigationService.CloseWindow<MainVM>();
        }

        [RelayCommand]
        public void ChangeCurrentViewModel(MenuItem menuItem)
        {
            if (menuItem.RequiredPermission != null)
            {
                // Vérifier si l'utilisateur possède la permission requise.
                if (!UserPermissions.Any(up => up.Permission_name == menuItem.RequiredPermission.Permission_name))
                {
                    CurrentViewModel = new MissPermissionVM();
                    return;
                }
            }
            if (menuItem.ViewModel != null)
                CurrentViewModel = menuItem.ViewModel;
        }

        private async Task LoadAvailablePermissions()
        {
            using var context = new ClimaDbContext();
            AvailablePermissions = await context.Permissions.ToArrayAsync();
        }
        private async Task LoadUserPermissions()
        {             
            using var context = new ClimaDbContext();

            var sessionUser = _sessionService.User;
            if (sessionUser == null)
            {
                // Tentative alternative : si ISessionService expose un UserId (décommentez/adaptez si besoin)
                // var userIdFromSession = (_sessionService as dynamic)?.UserId;+		ChangeCurrentViewModelCommand	{CommunityToolkit.Mvvm.Input.RelayCommand<invoice.Utilities.MenuItem>}	CommunityToolkit.Mvvm.Input.IRelayCommand<invoice.Utilities.MenuItem> {CommunityToolkit.Mvvm.Input.RelayCommand<invoice.Utilities.MenuItem>}

                // if (userIdFromSession == null) { _userPermissions = new List<Permission>(); return; }

                UserPermissions = new List<Permission>();
                return;
            }

            // Récupérer l'Id (utilise reflection/dynamic pour rester compatible avec différentes implémentations)
            int userId;
            try
            {
                userId = (int)sessionUser.GetType().GetProperty("UserId")!.GetValue(sessionUser)!;
            }
            catch
            {
                UserPermissions = new List<Permission>();
                return;
            }

            // Charger l'utilisateur avec son rôle et les permissions associées
            var userWithRole = await context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            UserPermissions = userWithRole?.Role?.RolePermissions
                .Select(rp => rp.Permission)
                .ToList() ?? new List<Permission>();
        }
    }
}
