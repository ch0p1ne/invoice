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
        private readonly ISessionService _sessionService;
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
            CurrentViewModel = new ExamenVM();

            MenuItems = new ObservableCollection<MenuItem>()
            {
               
               new MenuItem() {
                   Name = "Facturation",
                   Childrens =
                   [
                       new MenuItem("Apercu", null, null, "/Assets/icons/house.png"),
                       new MenuItem("Nouvelle Facture", null, new CreateFactureVM(), "/Assets/icons/add.png"),
                       new MenuItem("Vue des Factures", null, null, "/Assets/icons/list.png"),
                   ],
                   ViewModel = new NewInvoiceVM(),
                   Icon = "/Assets/icons/invoice.png"

               },
               new MenuItem() { Name = "Administration", ViewModel = new HomeVM(), Icon = "/Assets/icons/house.png" },
               new MenuItem()
               {
                   Name = "Gestions",
                   Childrens =
                   [
                     new MenuItem("Consultation", null, null, "/Assets/icons/answer.png"),
                     new MenuItem("Examen", null, new ExamenVM(), "/Assets/icons/exam.png"),
                     new MenuItem("Medecin", null, null, "/Assets/icons/online-appointment.png"),
                   ],
                   Icon = "/Assets/icons/file.png"
               },
            };
        }

        [RelayCommand]
        public void Logout()
        {
            _navigationService.NavigateTo<LoginVM>();
        }

        [RelayCommand]
        public void ChangeCurrentViewModel(VMBase parameter)
        {
            CurrentViewModel = parameter;
        }
    }
}
