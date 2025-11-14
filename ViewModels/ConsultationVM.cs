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
        public IEnumerable<MedicalConsultationType> MedicalConsultationType
        {
            get
            {
                // Convertit toutes les valeurs de l'enum en une collection
                return (MedicalConsultationType[])Enum.GetValues(typeof(MedicalConsultationType));
            }
        }
        private MedicalConsultationType _selectedMedicalConsultationType;
        public MedicalConsultationType SelectedMedicalConsultationType
        {
            get => _selectedMedicalConsultationType;
            set
            {
                SetProperty(ref _selectedMedicalConsultationType, value);
            }
        }

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

        private string _consultationName = string.Empty;
        public string ConsultationName
        {
            get => _consultationName;
            set
            { 
                SetProperty(ref _consultationName, InputValidator.ToUpperString(value) ?? string.Empty);
                CreateConsultationCommand.NotifyCanExecuteChanged();
                ClearConsultationCommand.NotifyCanExecuteChanged();
            }
        }
        private decimal _price = decimal.Zero;
        public decimal Price
        {
            get => _price;
            set
            { 
                SetProperty(ref _price, InputValidator.ValidPriceString(value));
                CreateConsultationCommand.NotifyCanExecuteChanged();
                ClearConsultationCommand.NotifyCanExecuteChanged();
            }
        }
        private string _reference = string.Empty;
        public string Reference
        {
            get => _reference;
            set
            {
                if (InputValidator.IsValidReferenceString(value))
                    SetProperty(ref _reference, InputValidator.ToUpperString(value) ?? string.Empty);
                CreateConsultationCommand.NotifyCanExecuteChanged();
                ClearConsultationCommand.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<Consultation> Consultations { get; set; } = new ObservableCollection<Consultation>();
        private Consultation _consultation = new();
        public Consultation Consultation
        {
            get => _consultation;
            set
            {
                SetProperty(ref _consultation, value);
                DeleteConsultationCommand.NotifyCanExecuteChanged();
            }
        }

        public ConsultationVM()
        {
            SelectedMedicalConsultationType = Utilities.MedicalConsultationType.MedecineDuTravail;
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
        [RelayCommand(CanExecute = nameof(CanExecuteSubmitModifie))]
        public async Task SubmitModifie(Consultation consultation)
        {
            using var context = new ClimaDbContext();
            context.Consultations.Update(consultation);

            await context.SaveChangesAsync();
            var MessageBoxService = new ModelOpenner("Modification correctement accomplie");
        }

        [RelayCommand(CanExecute = nameof(CanExecuteEditableExam))]
        public void EditableConsultation(Consultation consultation)
        {
            IsEditable = !IsEditable;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteCreateConsultation))]
        public async Task CreateConsultation()
        {
            using var context = new ClimaDbContext();
            Consultation.Categorie = SelectedMedicalConsultationType;
            Consultation.ConsultationName = ConsultationName;
            Consultation.Price = Price;
            Consultation.Reference = Reference;

            context.Consultations.Add(Consultation);

            await context.SaveChangesAsync();

            var MessageBoxService = new ModelOpenner("Création correctement accomplie");

            var dispatcher2 = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher2 != null && !dispatcher2.CheckAccess())
            {
                dispatcher2.Invoke(() =>
                {
                    Consultations.Add(Consultation);
                    ClearConsultation();
                    Consultation = new();
                    IsExpandableAddForm = false;
                });
            }
            else
            {
                Consultations.Add(Consultation);
                ClearConsultation();
                Consultation = new();
                IsExpandableAddForm = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteDeleteExam))]
        public async Task DeleteConsultation(Consultation consultation)
        {
            using var context = new ClimaDbContext();
            context.Consultations.Remove(consultation);
            await context.SaveChangesAsync();
            var MessageBoxService = new ModelOpenner("Suppression correctement accomplie");
            Consultations.Remove(consultation);
            IsEditable = false;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteClearConsultation))]
        public void ClearConsultation()
        {
            ConsultationName = string.Empty;
            Reference = string.Empty;
            Price = 0m;
        }



        // CanExecute method 
        public bool CanExecuteCreateConsultation()
        {
            return !(ConsultationName.Length < 2 || Reference.Length < 3 || Price < 0m);
        }
        public bool CanExecuteSubmitModifie(Consultation consultation)
        {
            if (consultation == null) return false;
            return !(consultation.ConsultationName == null 
                || consultation.ConsultationName.Length < 2 
                || consultation.Reference.Length < 3 
                || consultation.Price < 0m);
        }
        public bool CanExecuteDeleteExam(Consultation consultation)
        {
            return consultation != null;

        }
        public bool CanExecuteClearConsultation()
        {
            return ConsultationName.Length > 0 || Reference.Length > 0 || Price != 0m;
        }
        public bool CanExecuteEditableExam(Consultation consultation)
        {
            return consultation != null;
        }

    }
}
