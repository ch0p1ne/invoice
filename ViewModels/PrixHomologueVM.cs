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
    public partial class PrixHomologueVM : VMBase
    {
        private Categorie _categorie;
        public Categorie Categorie
        {
            get => _categorie;
            set => SetProperty(ref _categorie, value);
        }
        private ObservableCollection<Categorie> _categories;
        public ObservableCollection<Categorie>  Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
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
        private readonly string _title = "Prix Homologue";
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

        public ObservableCollection<PrixHomologue> PrixHomologues { get; set; } = new ObservableCollection<PrixHomologue>();
        public PrixHomologue PrixHomologue { get; set; } = new PrixHomologue();

        public PrixHomologueVM()
        {
            _categorie = new();
            _categories = new();
            _selectedCategorie = new();
            LoadPrixHomologuesAsync().ConfigureAwait(false);
            LoadCategoriesAsync().ConfigureAwait(false);
        }



        // A corriger car le Mvvm n'est pas respecter, utilisation de System.Windows...
        public async Task LoadPrixHomologuesAsync()
        {
            using var context = new ClimaDbContext();
            var PrixHomologuesList = await context.PrixHomologues.ToListAsync();

            // Mettre à jour la collection existante pour que les bindings voient les changements
            // Si ce code peut s'exécuter hors du thread UI, utiliser le Dispatcher.
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() =>
                {
                    PrixHomologues.Clear();
                    foreach (var e in PrixHomologuesList) PrixHomologues.Add(e);
                });
            }
            else
            {
                PrixHomologues.Clear();
                foreach (var e in PrixHomologuesList) PrixHomologues.Add(e);
            }
        }
        public async Task LoadCategoriesAsync()
        {
            using var context = new ClimaDbContext();
            var CategoriesList = await context.Categories.ToListAsync();

            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() =>
                {
                    Categories.Clear();
                    foreach (var e in CategoriesList) Categories.Add(e);
                });
            }
            else
            {
                Categories.Clear();
                foreach (var e in CategoriesList) Categories.Add(e);
            }

        }

        [RelayCommand]
        public void ChangeVisibility()
        {
            IsVisible = true;
        }
        [RelayCommand]
        public async Task SubmitModifie()
        {
            using var context = new ClimaDbContext();
            context.PrixHomologues.Update(PrixHomologue);

            await context.SaveChangesAsync();
            IsEditable = !IsEditable;
        }
        [RelayCommand]
        public void Editable()
        {
            IsEditable = !IsEditable;
        }
        [RelayCommand]
        public void AddPrixHomologuePage()
        {
            CurrentCrudOperation = "crudAdd";
        }

        [RelayCommand]
        public async Task CreatePrixHomologue()
        {
            using var context = new ClimaDbContext();
            // j'ai une autre option .... PrixHomologue.CategorieId = SelectedCategorie.CategorieID
            PrixHomologue.CategorieId = SelectedCategorie.CategorieId;

            context.PrixHomologues.Add(PrixHomologue);

            await context.SaveChangesAsync();

            var MessageBoxService = new ModelOpenner("Opération correctement accomplie");

            var dispatcher2 = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher2 != null && !dispatcher2.CheckAccess())
            {
                dispatcher2.Invoke(() =>
                {
                    IsExpandableAddForm = false;
                    PrixHomologues.Add(PrixHomologue);
                    CurrentCrudOperation = "crudList";
                });
            }
            else
            {
                IsExpandableAddForm = false;
                PrixHomologues.Add(PrixHomologue);
                CurrentCrudOperation = "crudList";
            }
        }
        [RelayCommand]
        public async Task CreateCategorie()
        {
            using var context = new ClimaDbContext();

            context.Categories.Add(Categorie);

            await context.SaveChangesAsync();

            var MessageBoxService = new ModelOpenner("Opération correctement accomplie");

            var dispatcher2 = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher2 != null && !dispatcher2.CheckAccess())
            {
                dispatcher2.Invoke(() =>
                {
                    Categories.Add(Categorie);
                });
            }
            else
            {
                Categories.Add(Categorie);
            }
        }

        [RelayCommand]
        public void ClearPrixHomologue()
        {
            PrixHomologue = new();
            IsExpandableAddForm = false;
        }

    }
}
