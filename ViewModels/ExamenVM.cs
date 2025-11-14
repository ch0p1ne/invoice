using CommunityToolkit.Mvvm.ComponentModel;
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

namespace invoice.ViewModels
{
    public partial class ExamenVM : VMBase
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
        private readonly string _title = "Examen";
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


        public ObservableCollection<Examen> Examens { get; set; } = new ObservableCollection<Examen>();
        private Examen _examen = new();
        public Examen Examen
        {
            get => _examen;
            set { SetProperty(ref _examen, value); DeleteExamCommand.NotifyCanExecuteChanged(); }
        }

        // Pour la validation
        private string _examenName = string.Empty;
        public string ExamenName
        {
            get => _examenName;
            set
            {
                SetProperty(ref _examenName, InputValidator.ToUpperString(value) ?? string.Empty);
                CreateExamenCommand.NotifyCanExecuteChanged();
                ClearExamenCommand.NotifyCanExecuteChanged();
            }
        }
        private string _reference = string.Empty;
        public string Reference
        {
            get => _reference;
            set
            {
                if (InputValidator.IsValidReferenceString(value))
                {
                    SetProperty(ref _reference, InputValidator.ToUpperString(value) ?? string.Empty);
                    CreateExamenCommand.NotifyCanExecuteChanged();
                    ClearExamenCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private decimal _price = decimal.Zero;
        public decimal Price
        {
            get => _price;
            set
            {
                SetProperty(ref _price, InputValidator.ValidPriceString(value));
                CreateExamenCommand.NotifyCanExecuteChanged();
                ClearExamenCommand.NotifyCanExecuteChanged();
            }
        }


        // Constructor
        public ExamenVM()
        {
            LoadExamensasync().ConfigureAwait(false);
        }



        // A corriger car le Mvvm n'est pas respecter, utilisation de System.Windows...
        public async Task LoadExamensasync()
        {
            using var context = new ClimaDbContext();
            var examensList = await context.Examens.ToListAsync();

            // Mettre à jour la collection existante pour que les bindings voient les changements
            // Si ce code peut s'exécuter hors du thread UI, utiliser le Dispatcher.
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() =>
                {
                    Examens.Clear();
                    foreach (var e in examensList) Examens.Add(e);
                });
            }
            else
            {
                Examens.Clear();
                foreach (var e in examensList) Examens.Add(e);
            }
        }

        [RelayCommand]
        public async Task SubmitModifie(Examen examen)
        {
            using var context = new ClimaDbContext();
            context.Examens.Update(examen);

            await context.SaveChangesAsync();
            var MessageBoxService = new ModelOpenner("Modification correctement accomplie");
            IsEditable = !IsEditable;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteEditableExam))]
        public void EditableExam(Examen examen)
        {
            IsEditable = !IsEditable;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteDeleteExam))]
        public async Task DeleteExam(Examen examen)
        {
            using var context = new ClimaDbContext();
            context.Examens.Remove(examen);
            await context.SaveChangesAsync();
            var MessageBoxService = new ModelOpenner("Suppression correctement accomplie");
            Examens.Remove(examen);
            IsEditable = false;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteCreateExamen))]
        public async Task CreateExamen()
        {
            using var context = new ClimaDbContext();
            Examen.ExamenName = ExamenName.Trim();
            Examen.Reference = Reference;
            Examen.Price = Price;

            context.Examens.Add(Examen);

            await context.SaveChangesAsync();

            var MessageBoxService = new ModelOpenner("Création correctement accomplie");

            var dispatcher2 = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher2 != null && !dispatcher2.CheckAccess())
            {
                dispatcher2.Invoke(() =>
                {
                    Examens.Add(Examen);
                    ClearExamen();
                    Examen = new Examen();
                    IsExpandableAddForm = false;
                });
            }
            else
            {
                Examens.Add(Examen);
                ClearExamen();
                Examen = new Examen();
                IsExpandableAddForm = false;
            }
        }


        [RelayCommand(CanExecute = nameof(CanExecuteClearExamen))]
        public void ClearExamen()
        {
            ExamenName = string.Empty;
            Reference = string.Empty;
            Price = 0m;
        }


        // CanExecute method 
        public bool CanExecuteCreateExamen()
        {
            return !(ExamenName.Length < 2 || Reference.Length < 3 || Price < 0m);
        }
        public bool CanExecuteSubmitModifie(Examen examen)
        {
            if(examen == null)
                return false;
            return !(examen.ExamenName == null || examen.ExamenName.Length < 2 || examen.Reference.Length < 3 || examen.Price < 0m);
        }
        public bool CanExecuteDeleteExam(Examen examen)
        {
            return examen != null;

        }
        public bool CanExecuteClearExamen()
        {
            return ExamenName.Length > 0 || Reference.Length > 0 || Price != 0m;
        }
        public bool CanExecuteEditableExam(Examen examen)
        {
            return examen != null;
        }
    }
}
