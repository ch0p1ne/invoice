using CommunityToolkit.Mvvm.Input;
using invoice.Context;
using invoice.Models;
using invoice.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Store;

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
        public ObservableCollection<Categorie> Categories
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

        // Pour la validation
        private string _elementName = string.Empty;
        public string ElementName
        {
            get => _elementName;
            set
            {
                SetProperty(ref _elementName, InputValidator.ToUpperString(value) ?? string.Empty);
                CreatePrixHomologueCommand.NotifyCanExecuteChanged();
                ClearPrixHomologueCommand.NotifyCanExecuteChanged();
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
                    CreatePrixHomologueCommand.NotifyCanExecuteChanged();
                    ClearPrixHomologueCommand.NotifyCanExecuteChanged();
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
                CreatePrixHomologueCommand.NotifyCanExecuteChanged();
                ClearPrixHomologueCommand.NotifyCanExecuteChanged();
            }
        }

        private string _categorieName = string.Empty;
        public string CategorieName
        {
            get => _categorieName;
            set
            {
                SetProperty(ref _categorieName, InputValidator.ToUpperString(value) ?? string.Empty);
                CreateCategorieCommand.NotifyCanExecuteChanged();
            }
        }



        //pour modificer une ligne du tableau
        private bool _isEditable = true;
        public bool IsEditable
        {
            get => _isEditable;
            set => SetProperty(ref _isEditable, value);
        }

        private Categorie? _selectedCategorie;
        public Categorie? SelectedCategorie
        {
            get => _selectedCategorie;
            set
            {
                SetProperty(ref _selectedCategorie, value);
                SubmitModifieCommand.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<PrixHomologue> PrixHomologues { get; set; } = new ObservableCollection<PrixHomologue>();
        private PrixHomologue _prixHomologue = new();
        public PrixHomologue PrixHomologue
        {
            get => _prixHomologue;
            set => SetProperty(ref _prixHomologue, value);
        }



        public PrixHomologueVM()
        {
            _categorie = new() { CategorieDescription = "-- Aucune --" };
            _categories = new();
            _selectedCategorie = null;
            LoadPrixHomologuesAsync().ConfigureAwait(false);
            LoadCategoriesAsync().ConfigureAwait(false);
        }



        // A corriger car le Mvvm n'est pas respecter, utilisation de System.Windows...
        public async Task LoadPrixHomologuesAsync()
        {
            using var context = new ClimaDbContext();
            var PrixHomologuesList = await context.PrixHomologues.Include(e => e.Categorie).ToListAsync();

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

        [RelayCommand(CanExecute = nameof(CanExecuteSubmitModifie))]
        public async Task SubmitModifie(PrixHomologue prixHomologue)
        {
            using var context = new ClimaDbContext();
            prixHomologue.CategorieId = (int)(SelectedCategorie?.CategorieId!);
            context.PrixHomologues.Update(prixHomologue);

            await context.SaveChangesAsync();
            var MessageBoxService = new ModelOpenner("Modification correctement accomplie");
        }
        [RelayCommand(CanExecute = nameof(CanExecuteEditablePrixHomologue))]
        public void EditablePrixHomologue(PrixHomologue prixHomologue)
        {
            IsEditable = !IsEditable;
        }
        [RelayCommand(CanExecute = nameof(CanExecuteCreatePrixHomologue))]
        public async Task CreatePrixHomologue()
        {
            using var context = new ClimaDbContext();
            PrixHomologue.ElementName = ElementName;
            PrixHomologue.Reference = Reference;
            PrixHomologue.Price = Price;
            PrixHomologue.CategorieId = SelectedCategorie.CategorieId;
            PrixHomologue.Categorie = null;

            context.PrixHomologues.Add(PrixHomologue);

            await context.SaveChangesAsync();

            var MessageBoxService = new ModelOpenner("Opération correctement accomplie");

            var dispatcher2 = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher2 != null && !dispatcher2.CheckAccess())
            {
                dispatcher2.Invoke(() =>
                {
                    PrixHomologues.Add(PrixHomologue);
                    ClearPrixHomologue();
                    PrixHomologue = new();
                    IsExpandableAddForm = false;
                });
            }
            else
            {
                PrixHomologues.Add(PrixHomologue);
                ClearPrixHomologue();
                PrixHomologue = new();
                IsExpandableAddForm = false;
            }
        }
        [RelayCommand(CanExecute = nameof(CanExecuteDeletePrixHomologue))]
        public async Task DeletePrixHomologue(PrixHomologue prixHomologue)
        {
            using var context = new ClimaDbContext();
            context.PrixHomologues.Remove(prixHomologue);
            await context.SaveChangesAsync();
            var MessageBoxService = new ModelOpenner("Suppression correctement accomplie");
            PrixHomologues.Remove(prixHomologue);
            IsEditable = true;
        }
        [RelayCommand(CanExecute = nameof(CanExecuteCreateCategorie))]
        public async Task CreateCategorie()
        {
            using var context = new ClimaDbContext();
            Categorie.CategorieName = CategorieName;
            context.Categories.Add(Categorie);

            await context.SaveChangesAsync();

            var MessageBoxService = new ModelOpenner("Opération correctement accomplie");

            var dispatcher2 = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher2 != null && !dispatcher2.CheckAccess())
            {
                dispatcher2.Invoke(() =>
                {
                    Categories.Add(Categorie);
                    Categorie = new Categorie() { CategorieDescription = "-- Aucune --" };
                });
            }
            else
            {
                Categories.Add(Categorie);
                Categorie = new Categorie() { CategorieDescription = "-- Aucune --" };
            }
        }
        [RelayCommand(CanExecute = nameof(CanExecuteClearPrixHomologue))]
        public void ClearPrixHomologue()
        {
            ElementName = string.Empty;
            Reference = string.Empty;
            Price = decimal.Zero;
            SelectedCategorie = null;
        }


        // CanExecute method 
        public bool CanExecuteCreatePrixHomologue()
        {
            return !(ElementName.Length < 2 || Reference.Length < 3 || Price < 0m || SelectedCategorie == null);
        }
        public bool CanExecuteSubmitModifie(PrixHomologue PrixHomologue)
        {
            if (PrixHomologue == null || SelectedCategorie == null)
                return false;
            return !(PrixHomologue.ElementName == null || PrixHomologue.ElementName.Length < 2 || PrixHomologue.Reference.Length < 3 || PrixHomologue.Price < 0m);
        }
        public bool CanExecuteDeletePrixHomologue(PrixHomologue PrixHomologue)
        {
            return PrixHomologue != null;

        }
        public bool CanExecuteClearPrixHomologue()
        {
            return ElementName.Length > 0 || Reference.Length > 0 || Price != 0m;
        }
        public bool CanExecuteEditablePrixHomologue(PrixHomologue PrixHomologue)
        {
            return PrixHomologue != null;
        }
        public bool CanExecuteCreateCategorie()
        {
            return !(CategorieName == null || CategorieName.Length < 3);
        }
    }
}
