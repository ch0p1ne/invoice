using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using invoice.Services;
using invoice.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice.ViewModels
{
    public partial class MainVM : VMBase
    {
        private ISessionService _sessionService;
        private readonly INavigationService _navigationService;
        public ObservableCollection<MenuItem> MenuItems { get; }

        private readonly DateOnly _dateOnly = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly DateOnly => _dateOnly;



        public ISessionService SessionService => _sessionService;
        [ObservableProperty]
        private VMBase _currentViewModel;


        public MainVM(INavigationService navigationService, ISessionService sessionService)
        {
            _navigationService = navigationService;
            _sessionService = sessionService;
            CurrentViewModel = new CreateFactureVM(sessionService);

            MenuItems = new ObservableCollection<MenuItem>()
            {
               
               new MenuItem() {
                   Name = "Facturation",
                   Childrens =
                   [
                       new MenuItem("Apercu", null, null, "/Assets/icons/house.png"),
                       new MenuItem("Nouvelle Facture", null, new CreateFactureVM(sessionService), "/Assets/icons/add.png"),
                       new MenuItem("Vue des Factures", null, null, "/Assets/icons/list.png"),
                   ],
                   Icon = "/Assets/icons/invoice.png"

               },
               new MenuItem() { 
                   Name = "Administration", 
                   ViewModel = new HomeVM(), 
                   Icon = "/Assets/icons/house.png" ,
                   Childrens =
                   [
                     new MenuItem("Utilisateurs et Rôles", null, new UserAndRoleVM(), "/Assets/icons/users.png")
                     //new MenuItem("Médecins", null, new MedecinVM(), "/Assets/icons/doctor.png"),
                     //new MenuItem("Assurances", null, new AssuranceVM(), "/Assets/icons/insurance.png"),
                   ]
               },
               new MenuItem()
               {
                   Name = "Gestions",
                   Childrens =
                   [
                     new MenuItem("Consultation", null, new ConsultationVM(), "/Assets/icons/answer.png"),
                     new MenuItem("Examen", null, new ExamenVM(), "/Assets/icons/exam.png"),
                     new MenuItem("Prix Homologue", null, new PrixHomologueVM(), "/Assets/icons/online-appointment.png"),
                   ],
                   Icon = "/Assets/icons/file.png"
               },
            };
        }

        [RelayCommand]
        public void Logout()
        {
            _navigationService.NavigateTo<LoginVM>();
            _navigationService.CloseWindow<MainVM>();
        }

        [RelayCommand]
        public void ChangeCurrentViewModel(VMBase parameter)
        {
            CurrentViewModel = parameter;
        }
    }
}
