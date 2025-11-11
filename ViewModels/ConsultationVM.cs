using CommunityToolkit.Mvvm.Input;
using invoice.Context;
using invoice.Models;
using invoice.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace invoice.ViewModels
{
    public partial class ConsultationVM : VMBase
    {
        private bool _isExpandableAddForm = false;
        public bool IsExpandableAddForm
        {
            get => _isExpandableAddForm;
            set
            {
                SetProperty(ref _isExpandableAddForm, value);
            }
        }
        private readonly string _title = "Consultation";
        public string Title
        {
            get => _title;
        }

        //pour modificer une ligne du tableau
        private bool _isEditable = true;
        public bool IsEditable
        {
            get => _isEditable;
            set => SetProperty(ref _isEditable, value);
        }
        private bool _isVisible = false;
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }
        private string _currentCrudOperation = "crudList";
        public string CurrentCrudOperation
        {
            get => _currentCrudOperation;
            set
            {
                _currentCrudOperation = value;
                OnPropertyChanged(nameof(CurrentCrudOperation));
            }
        }
        private Categorie _selectedCategorie;
        public Categorie SelectedCategorie
        {
            get => _selectedCategorie;
            set => SetProperty(ref _selectedCategorie, value);
        }

        public ObservableCollection<Consultation> Consultations { get; set; } = new ObservableCollection<Consultation>();
        private Consultation _consultation = new();
        public Consultation Consultation
        {
            get => _consultation;
            set => SetProperty(ref _consultation, value);
        }

        public ConsultationVM()
        {
            _selectedCategorie = new();
            LoadConsultationsAsync().ConfigureAwait(false);
        }



        // A corriger car le Mvvm n'est pas respecter, utilisation de System.Windows...
        public async Task LoadConsultationsAsync()
        {
            using var context = new ClimaDbContext();
            var ConsultationsList = await context.Consultations.ToListAsync();

            // Mettre à jour la collection existante pour que les bindings voient les changements
            // Si ce code peut s'exécuter hors du thread UI, utiliser le Dispatcher.
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() =>
                {
                    Consultations.Clear();
                    foreach (var e in ConsultationsList) Consultations.Add(e);
                });
            }
            else
            {
                Consultations.Clear();
                foreach (var e in ConsultationsList) Consultations.Add(e);
            }
        }
        [RelayCommand]
        public async Task SubmitModifie()
        {
            // TO DO !!! Les imformations qui sont dans les formulaire NE SONT PAS liée 
            // à Consultation mais plutot a un selectedItem !!!, il faut que je prennent
            // ces données et que j'initialise Consultation avec.
            using var context = new ClimaDbContext();
            context.Consultations.Update(Consultation);

            await context.SaveChangesAsync();
            IsEditable = !IsEditable;
            ChangeVisibility();
        }
        [RelayCommand]
        public void ChangeVisibility()
        {
            //IsVisible = !IsVisible;
            // TEST
            IsVisible = false;
        }

        [RelayCommand]
        public void Editable()
        {
            IsEditable = !IsEditable;
        }
        [RelayCommand]
        public void AddConsultationPage()
        {
            CurrentCrudOperation = "crudAdd";
        }

        [RelayCommand]
        public async Task CreateConsultation()
        {
            using var context = new ClimaDbContext();

            context.Consultations.Add(Consultation);

            await context.SaveChangesAsync();

            var MessageBoxService = new ModelOpenner("Opération correctement accomplie");

            var dispatcher2 = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher2 != null && !dispatcher2.CheckAccess())
            {
                dispatcher2.Invoke(() =>
                {
                    Consultations.Add(Consultation);
                    Consultation = new();
                });
            }
            else
            {
                Consultations.Add(Consultation);
                Consultation = new();
            }
        }

        [RelayCommand]
        public void ClearConsultation()
        {
            Consultation = new();
            IsExpandableAddForm = false;
        }

    }
}
