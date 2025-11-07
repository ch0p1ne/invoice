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
        public ObservableCollection<Examen> Examens { get; set; } = new ObservableCollection<Examen>();
        public Examen Examen { get; set; } = new Examen();

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
        public void ChangeVisibility()
        {
            IsVisible = true;
        }
        [RelayCommand]
        public async Task SubmitModifie(Examen examen)
        {
            using var context = new ClimaDbContext();
            context.Examens.Update(examen);

            await context.SaveChangesAsync();
            IsEditable = !IsEditable;
        }
        [RelayCommand]
        public void Editable()
        { 
            IsEditable = !IsEditable;
        }
        [RelayCommand]
        public void AddExamenPage()
        {
            CurrentCrudOperation = "crudAdd";
        }

        [RelayCommand]
        public async Task CreateExamen()
        {
            using var context = new ClimaDbContext();

            context.Examens.Add(Examen);

            await context.SaveChangesAsync();

            var MessageBoxService = new ModelOpenner("Opération correctement accomplie");

            var dispatcher2 = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher2 != null && !dispatcher2.CheckAccess())
            {
                dispatcher2.Invoke(() =>
                {
                    Examens.Add(Examen);
                    CurrentCrudOperation = "crudList";
                    ClearExamen();
                });
            }
            else
            {
                Examens.Add(Examen);
                CurrentCrudOperation = "crudList";
                ClearExamen();
            }
        }

        [RelayCommand]
        public void UndoAddExamen()
        {
            CurrentCrudOperation = "crudList";
        }

        [RelayCommand]
        public void ClearExamen()
        {
            Examen.ExamenName = "";
            Examen.Reference = "";
            Examen.Price = 0;
            IsExpandableAddForm = false;
        }

    }
}
