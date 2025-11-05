using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using invoice.Context;
using invoice.Models;
using invoice.Services;
using invoice.Utilities;
using invoice.Utilities.PdfGenerator;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace invoice.ViewModels
{
    public partial class CreateFactureVM : VMBase
    {
        private readonly string _title = "Facture";
        private bool _facTypeSet = false;
        private string _currentPartOfNewFacture = "crudCreateOne";
        private Examen? _selectedAvailableExam;
        private Patient? _selectedPatient;
        public Assurance? _selectedAssurance;
        private bool _selectedDiscountPercent = true;
        private bool _selectedDiscountFlat = false;
        private int _selectedQty = 1;
        private double _totalHTPrice = 0;
        private double _totalTTCPrice = 0;
        private double _taxe = 0; // TVA 20%
        private decimal _css = 0.10m; // TVA 20%
        private bool _patientDefined = false;
        private Patient? _patient = new Patient();
        private FactureExamen? _factureExamen;
        private bool _isInsurance = false;
        private double _discountPercent;
        private string _discountType = "Percent";
        private string _paymentMethod = "Espèces";
        private bool _generatePDFButtonIsEnable = false;
        private double _amountPaid = 0;



        // Contructor

        public CreateFactureVM(ISessionService sessionService)
        {
            SessionService = sessionService;
            LoadPatientsList().ConfigureAwait(false);
            LoadAssuranceList().ConfigureAwait(false);
            _patient.DateOfBirth = DateTime.Now;
        }

        //Property
        public ISessionService SessionService { get; set; }
        public Facture Facture { get; set; }
        public bool GeneratePDFButtonIsEnable 
        {
            get => _generatePDFButtonIsEnable;
            set =>SetProperty(ref _generatePDFButtonIsEnable, value);
        }
        public double AmountPaid
        {
            get => _amountPaid;
            set => SetProperty(ref _amountPaid, value);
        }
        public double DiscountPercent
        {
            get => _discountPercent;
            set
            {
                value = value / 100;
                SetProperty(ref _discountPercent, value);
            }
        }
        public string PaymentMethod
        {
            get => _paymentMethod;
            set => SetProperty(ref _paymentMethod, value);
        }
        public Assurance? SelectedAssurance
        {
            get => _selectedAssurance;
            set => SetProperty(ref _selectedAssurance, value);
        }
        public bool IsInsurance
        {
            get => _isInsurance;
            set => SetProperty(ref _isInsurance, value);
        }
        public string Title
        {
            get => _title;
        }
        public string CurrentPartOfNewFacture
        {
            get => _currentPartOfNewFacture;
            set
            {
                _currentPartOfNewFacture = value;
                OnPropertyChanged();
            }
        }
        public Patient? Patient 
        {
            get => _patient;
            set => SetProperty(ref _patient, value);
        }
        public FactureExamen FactureExamen
        {
            get =>  _factureExamen;
            set => SetProperty(ref _factureExamen, value);
        }
        public double Taxe
        {
            get => _taxe;
            set => SetProperty(ref _taxe, value);
        }
        public string DiscountType
        {
            get => _discountType;
            set => SetProperty(ref _discountType, value);
        }
        public bool SelectedDiscountPercent
        {
            get => _selectedDiscountPercent;
            set
            {
                SetProperty(ref _selectedDiscountPercent, value);
                if((bool)value)
                    DiscountType = "Percent";
            }
        }
        public bool SelectedDiscountFlat
        {
            get => _selectedDiscountFlat;
            set { 
                SetProperty(ref _selectedDiscountFlat, value); 
                if((bool)value)
                    DiscountType = "Flat"; }
        }

        public FactureExamen FactureExam { get; set; }
        public Patient SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                if (_selectedPatient != value)
                {
                    _selectedPatient = value;
                    OnPropertyChanged(nameof(SelectedPatient));

                    // 💡 LOGIQUE D'ACTIVATION :
                    // La sélection du patient (value) est non-null si un élément a été choisi.

                    if (value != null)
                    {
                        PatientDefined = true;
                        Patient = value;
                        IsInsurance =  Patient.AssuranceNumber is not null ?  true :  false;
                        foreach(var assurance in Assurances)
                        {
                            if (assurance.AssuranceId == Patient.AssuranceId)
                                SelectedAssurance = assurance;
                            else
                                SelectedAssurance = null;
                        }
                    }
                    else
                    {
                        PatientDefined = false;
                        Patient = new Patient();
                    }
                }
            }
        }
        public bool PatientDefined
        {
            get => _patientDefined;
            set => SetProperty(ref _patientDefined, value); 
        }
        public double TotalHTPrice
        {
            get => _totalHTPrice;
            set => SetProperty(ref _totalHTPrice, value);
        }
        public double TotalTTCPrice
        {
            get => _totalTTCPrice;
            set => SetProperty(ref _totalTTCPrice, value);
        }
        public decimal Css 
        { 
            get => _css;
            set => SetProperty(ref _css, value);
        }
        public ObservableCollection<Examen> AvailableExamens { get; set; } = new ObservableCollection<Examen>();
        public Examen? SelectedAvailableExam
        { get => _selectedAvailableExam;
            set
            {
                if (_selectedAvailableExam != value)
                {
                    _selectedAvailableExam = value;
                    OnPropertyChanged(nameof(SelectedAvailableExam));
                    AddExamCommand.NotifyCanExecuteChanged();
                }
            }
        }
        public ObservableCollection<int> QtyAvailable { get; set; } = new ObservableCollection<int>(Enumerable.Range(1, 10));
        public int SelectedQty 
        {
            get => _selectedQty;
            set => SetProperty(ref _selectedQty, value);
        }
        // Objet principal pour faire des facture
        public ObservableCollection<InvoiceExam> InvoiceExams { get; set; } =  [];
        public ObservableCollection<Patient> Patients { get; set; } = [];
        public ObservableCollection<Assurance> Assurances { get; set; } = [];



        // Command
        [RelayCommand]
        public void ChangeCurrentView(string viewName)
        {
            CurrentPartOfNewFacture = viewName;
            GetExamenList().ConfigureAwait(false);
        }
        [RelayCommand(CanExecute = nameof(CanExecuteAddExam))]
        public void AddExam()
        {
            if (SelectedAvailableExam != null)
            {
                
                InvoiceExams.Add( new InvoiceExam { Exam = SelectedAvailableExam, Qty = SelectedQty });

                SelectedAvailableExam = null;
                CalculAllIndexedPrice();
            }
        }
        [RelayCommand]
        public void removeInvoiceExam(object parameter)
        {
            if (parameter is InvoiceExam invoiceExamnsToRemove)
            {
                InvoiceExams.Remove(invoiceExamnsToRemove);
                CalculAllIndexedPrice();
            }
        }
        
        
        //  H A R D - H A R D
        // Fonction à comprendre absolument
        [RelayCommand]
        public async Task CreateInvoice()
        {
            if (!InvoiceExams.Any())
            {
                throw new InvalidOperationException("Impossible de créer une facture sans examens.");
            }

            // Utilisation d'une transaction pour garantir l'atomicité (si un des SaveAsync échoue)
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using var context = new ClimaDbContext();

            // --- 1. CRÉATION DE L'ENTÊTE FACTURE ---

            var nouvelleFacture = new Facture
            {
                Reference = await GenerateReference(),
                Type = InvoiceType.Examen,
                TotalAmountHT = (decimal?)TotalHTPrice,
                Tva = (decimal)Taxe,
                Css = Css,
                InsuranceCoveragePercent = SelectedAssurance?.CoveragePercent,
                PatientPercent = SelectedAssurance != null ? (1m - SelectedAssurance.CoveragePercent) : 1m,
                AmountPaid = (decimal)AmountPaid,
                DiscountPercent = (decimal)DiscountPercent,
                Status = StatusType.Non_payer,
                PaymentMethod = PaymentMethod,

                // Assurez-vous que les objets Patient et User existent ou que leurs IDs sont définis
                PatientId = Patient!.PatientId,
                UserId = SessionService.User!.UserId,
            };

            context.Factures.Add(nouvelleFacture);

            // --- 2. PREMIER SAVE : Obtention de FactureId ---

            // Ceci enregistre la facture et FACTUREId est généré par la DB et affecté à nouvelleFacture.FactureId
            await context.SaveChangesAsync();

            // --- 3. CRÉATION ET AJOUT DES LIGNES DE JOINTURE (FactureExamen) ---

            var lignesFactureAAjouter = new List<FactureExamen>();

            foreach (var ligneVM in InvoiceExams)
            {
                // a. L'Examen doit être considéré comme existant (Unchanged)
                // Ceci évite à EF Core d'essayer de l'insérer à nouveau et valide la clé étrangère.
                context.Entry(ligneVM.Exam).State = EntityState.Unchanged;

                // b. Créer la nouvelle entité de jointure
                var ligneFactureExamen = new FactureExamen
                {
                    // Les IDs sont maintenant disponibles
                    FactureId = nouvelleFacture.FactureId,
                    ExamenId = ligneVM.Exam.ExamenId,

                    Qte = ligneVM.Qty,
                };

                lignesFactureAAjouter.Add(ligneFactureExamen);
            }

            // c. Ajout des entités de jointure au contexte
            context.FacturesExamens.AddRange(lignesFactureAAjouter);

            // --- 4. DEUXIÈME SAVE : Enregistrement des Lignes N:N ---

            await context.SaveChangesAsync();

            // Si toutes les opérations ont réussi, complétez la transaction
            scope.Complete();
            InvoiceExams.Clear();
            Facture = nouvelleFacture;
            GeneratePDFButtonIsEnable = true;
            GenererFacturePdf();
            var Messagebox = new ModelOpenner($"Création de la facture {nouvelleFacture.Reference} terminé");
        }
        
        
        
        [RelayCommand]
        public async Task CreatePatient()
        {
            try
            {
                using var context = new ClimaDbContext();
                if (IsInsurance)
                {
                    if(SelectedAssurance != null)
                        if( Patient != null )
                            if(Patient.AssuranceNumber is not null)
                                Patient.AssuranceId = SelectedAssurance.AssuranceId;
                }

                context.Patients.Add(Patient!);
                await context.SaveChangesAsync();
                PatientDefined = true;
                await LoadPatientsList();
                var MessageBox = new ModelOpenner("Utilisateur Ajouter correctement ajouter!");
            }
            catch (Exception ex)
            {

                throw new System.InvalidOperationException("Erreur lors de la création du patient", ex);
            }
        }
        [RelayCommand]
        public void GenerateInvoicePDF()
        {
            
        }


        // Method
        public async Task<string> GenerateReference()
        {
            string reference;
            int currentDay = DateTime.Now.Day;
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            try
            {

                using var context = new ClimaDbContext();

                // Dans le cas ou il ya un immense nombre de facture, on va prendre la
                // reference de la derniere et l'icrementer
                var factureCount = await context.Factures.CountAsync();
                reference = "FAC-" + factureCount +  currentDay + currentMonth + currentYear;

                return reference;
            }
            catch (Exception ex)
            {
                throw new System.InvalidOperationException($"Impossible de faire une facture : {ex}", ex);
            }
        }
        public void CalculAllIndexedPrice()
        {
            // Calcul du total HT
            double total = 0;
            foreach (var item in InvoiceExams)
            {
                total += (double)(item.Exam.Price * item.Qty);
            }

            TotalHTPrice = total;

            TotalTTCPrice = TotalHTPrice + (TotalHTPrice * Taxe); // TVA 20% ?
        }
        public async Task GetExamenList()
        {
            try
            {
                using var context = new ClimaDbContext();
                var examensList = await context.Examens.ToListAsync();
                AvailableExamens = new ObservableCollection<Examen>(examensList);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task LoadPatientsList()
        {
            using var context = new ClimaDbContext();
            var patientsList = await context.Patients.ToListAsync();

            // Mettre à jour la collection existante pour que les bindings voient les changements
            // Si ce code peut s'exécuter hors du thread UI, utiliser le Dispatcher.
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() =>
                {
                    Patients.Clear();
                    foreach (var e in patientsList) Patients.Add(e);
                });
            }
            else
            {
                Patients.Clear();
                foreach (var e in patientsList) Patients.Add(e);
            }
        }
        public async Task LoadAssuranceList()
        {
            try
            {
                using var context = new ClimaDbContext();
                var assuranceList = await context.Assurances.ToListAsync();

                // Mettre à jour la collection existante pour que les bindings voient les changements

                var dispatcher = System.Windows.Application.Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(() =>
                    {
                        Assurances.Clear();
                        foreach (var e in assuranceList) Assurances.Add(e);
                    });
                }
                else
                {
                    Assurances.Clear();
                    foreach (var e in assuranceList) Assurances.Add(e);
                }
            }
            catch (Exception ex)
            {

                throw new System.InvalidOperationException("Erreur lors du chargement des assurances", ex);
            }
        }
        private bool CanExecuteAddExam()
        {
            // Le bouton est actif SEULEMENT si un examen est sélectionné dans le ComboBox
            return SelectedAvailableExam != null;
        }
        public void GenererFacturePdf()
        {
            var folderPath = "c:/clima-g/factures/";
            var fileName = $"{Facture.Reference}_{Patient.FirstName}_{Patient.LastName}.pdf";

            // 1. Définir le chemin complet du fichier
            string filePath = Path.Combine(folderPath, fileName);
            // Vérifie si le dossier n'existe PAS
            if (!Directory.Exists(folderPath))
            {
                // Crée tous les répertoires et sous-répertoires dans le chemin spécifié.
                // Cette méthode est sûre, même si le chemin contient plusieurs niveaux non existants.
                Directory.CreateDirectory(folderPath); // ✨ Le dossier est créé ici

                Console.WriteLine($"Dossier créé : {folderPath}");
            }

            // Crée le document
            var document = new FactureDocument(Facture, Patient);

            // Génère le PDF et l'ouvre
            document.GeneratePdf(filePath);
        }

    }




    // Facture intermédiaire classe
    public class InvoiceExam : ObservableObject
    {
        private Examen _exam;
        private int _qty;
        public Examen Exam 
        {
            get => _exam;
            set => SetProperty(ref _exam, value);
        }


        public int Qty
        { 
            get => _qty;
            set => SetProperty(ref _qty, value);
        }
    }
}
