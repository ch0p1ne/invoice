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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using Windows.Data.Pdf;

namespace invoice.ViewModels
{
    public partial class CreateFactureVM : VMBase
    {
        private const decimal CONST_TPS = 0.09m;
        private const decimal CONST_CSS = 0.01m;


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
        private decimal _css = CONST_CSS; // TVA 20%
        private decimal _tps = CONST_TPS;
        private bool _patientDefined = false;
        private Patient? _patient = new Patient();
        private FactureExamen? _factureExamen;
        private bool _isInsurance = false;
        private double _discountPercent = 0;
        private double _discountFlat = 0;
        private string _discountType = "Percent";
        private PaymentMethod _paymentMethod = Utilities.PaymentMethod.Especes;
        private bool _generatePDFButtonIsEnable = false;
        private double _amountPaid = 0;
        private string _facturePdfPath = string.Empty;
        private string _lastName = string.Empty;
        private string _firstName = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _phoneNumber2 = string.Empty;
        private DateTime? _dateOfBirth;
        private decimal _netApayer = decimal.Zero;
        private decimal _amountLeft = decimal.Zero;
        private bool _showAdvanceInvoiceParam = false;
        private bool _isCssCheck = true;
        private bool _isTPSCheck = true;
        private bool _isShowAmountLeft = false;
        private double _calculateCss = 0;
        private double _calculateTPS = 0;



        // Contructor

        public CreateFactureVM(ISessionService sessionService)
        {
            SessionService = sessionService;
            _ = LoadPatientsList();
        }

        //Property
        private string FacturePdfPath
        { 
            get => _facturePdfPath;
            set
            {
                SetProperty(ref _facturePdfPath, value);
                PreviewInvoiceCommand.NotifyCanExecuteChanged();
            }
        }
        public ISessionService SessionService { get; set; }
        public Facture Facture { get; set; }
        public bool GeneratePDFButtonIsEnable 
        {
            get => _generatePDFButtonIsEnable;
            set =>SetProperty(ref _generatePDFButtonIsEnable, value);
        }
        public decimal NetAPayer
        {
            get => _netApayer;
            set
            {
                SetProperty(ref _netApayer, value);
            }
        }
        public double AmountPaid
        {
            get => _amountPaid;
            set
            {
                SetProperty(ref _amountPaid, value);
                CalculAllIndexedPrice();
                if(_amountPaid > 0)
                    IsShowAmountLeft = true;
                else IsShowAmountLeft = false;
            }
        }
        public decimal AmountLeft
        {
            get => _amountLeft;
            set
            {
                SetProperty(ref _amountLeft, value);
            }
        }
        public double DiscountPercent
        {
            get => _discountPercent;
            set
            {
                value = value / 100;
                SetProperty(ref _discountPercent, value);
                CalculAllIndexedPrice();
            }
        }
        public PaymentMethod PaymentMethod
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
                {
                    _selectedDiscountFlat = false;
                    DiscountType = "Percent";
                    DiscountFlat = 0;
                }
            }
        }
        public bool SelectedDiscountFlat
        {
            get => _selectedDiscountFlat;
            set 
            { 
                SetProperty(ref _selectedDiscountFlat, value); 
                if ((bool)value)
                {
                    _selectedDiscountPercent = false;
                    DiscountType = "Flat";
                    DiscountPercent = 0;
                }

                }
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
                    LastName = _selectedPatient?.LastName ?? string.Empty;
                    FirstName = _selectedPatient?.FirstName ?? string.Empty;
                    PhoneNumber1 = _selectedPatient?.PhoneNumber ?? string.Empty;
                    PhoneNumber2 = _selectedPatient?.PhoneNumber2 ?? string.Empty;
                    DateOfBirth = _selectedPatient?.DateOfBirth;
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
                    AddInvoiceExamCommand.NotifyCanExecuteChanged();
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
        public string LastName
        { 
            get => _lastName;
            set
            {
                SetProperty(ref _lastName, InputValidator.ToUpperString(value) ?? value);
                CreatePatientCommand.NotifyCanExecuteChanged();
            }
        }
        public string FirstName 
        { 
            get => _firstName;
            set
            {
                SetProperty(ref _firstName, InputValidator.CapitalizeEachWord(value) ?? value);
                CreatePatientCommand.NotifyCanExecuteChanged();
            }
        }
        public string PhoneNumber1
        { 
            get => _phoneNumber;
            set
            {
                SetProperty(ref _phoneNumber, InputValidator.FormatAndValidateInput(value) ?? value);
                CreatePatientCommand.NotifyCanExecuteChanged();
            }
        }
        public string PhoneNumber2
        { 
            get => _phoneNumber2;
            set
            {
                SetProperty(ref _phoneNumber2, InputValidator.FormatAndValidateInput(value) ?? value);
                CreatePatientCommand.NotifyCanExecuteChanged();
            }
        }
        public DateTime? DateOfBirth
        { 
            get => _dateOfBirth;
            set
            {
                SetProperty(ref _dateOfBirth, value);
                CreatePatientCommand.NotifyCanExecuteChanged();
            }
        }

        public double DiscountFlat 
        { 
            get => _discountFlat;
            set
            {
                SetProperty(ref _discountFlat, value);
                CalculAllIndexedPrice();
            }
        }
        public bool ShowAdvanceInvoiceParam
        {            
            get => _showAdvanceInvoiceParam;
            set => SetProperty(ref _showAdvanceInvoiceParam, value);
        }
        public decimal TPS
        { 
            get => _tps;
            set => SetProperty(ref _tps, value);
        }
        public bool IsCssCheck
        { get => _isCssCheck; set { SetProperty(ref _isCssCheck, value); CalculAllIndexedPrice(); } }
        public bool IsTPSCheck
        { get => _isTPSCheck; set { SetProperty(ref _isTPSCheck, value); CalculAllIndexedPrice(); } }
        public double CalculateCss
        { get => _calculateCss; set => SetProperty(ref _calculateCss, value); }
        public double CalculateTPS 
        { get => _calculateTPS; set => SetProperty(ref _calculateTPS, value); }
        public bool IsShowAmountLeft { get => _isShowAmountLeft; set => SetProperty(ref _isShowAmountLeft, value); }



        // Command
        [RelayCommand]
        public void ChangeCurrentView(string viewName)
        {
            CurrentPartOfNewFacture = viewName;
            DiscountPercent = 0;
            DiscountFlat = 0;
            AmountPaid = 0;
            if(viewName == "crudCreateOne")
            {
                PatientDefined = false;
                Patient = new Patient();

            }
            ShowAdvanceInvoiceParam = false;
            InvoiceExams.Clear();

            // TO DO : Load data if needed
            if(viewName == "CrudCreateTwo")
                _ = GetExamenList();
        }
        [RelayCommand(CanExecute = nameof(CanExecuteAddInvoiceExam))]
        public void AddInvoiceExam()
        {
            if (SelectedAvailableExam != null)
            {

                    //if(InvoiceExams.Contains(new InvoiceExam { Exam = SelectedAvailableExam, Qty = SelectedQty }))

                foreach(var item in InvoiceExams)
                {
                    if(item.Exam == SelectedAvailableExam)
                    {
                        item.Qty += SelectedQty;
                        SelectedAvailableExam = null;
                        CalculAllIndexedPrice();
                        return;
                    }
                }

                InvoiceExams.Add( new InvoiceExam { Exam = SelectedAvailableExam, Qty = SelectedQty });

                SelectedAvailableExam = null;
                CalculAllIndexedPrice();
                ShowAdvanceInvoiceParam = true;
            }
            CreateInvoiceCommand.NotifyCanExecuteChanged();
        }
        [RelayCommand]
        public void removeInvoiceExam(object parameter)
        {
            if (parameter is InvoiceExam invoiceExamnsToRemove)
            {
                InvoiceExams.Remove(invoiceExamnsToRemove);
                CalculAllIndexedPrice();
            }
            CreateInvoiceCommand.NotifyCanExecuteChanged();
            if(InvoiceExams.Count == 0)
            {
                ShowAdvanceInvoiceParam = false;
            }
        }
        
        
        //  H A R D - H A R D
        // Fonction à comprendre absolument
        [RelayCommand(CanExecute = nameof(CanExecuteCreateFact))]
        public async Task CreateInvoice()
        {
            var messageBox = new ModelOpenner();
            try
            {
                if (messageBox.Show("Création de la facture en cours...", "Voulez vous vraiment établir une facture ?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return; // L'utilisateur a annulé l'opération
                }

                if (!InvoiceExams.Any())
                {
                    messageBox.Show("Erreur critique", "Une erreur critique est survenue, fermeture...", MessageBoxButton.OK);
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
                    TotalAmountHT = (decimal)TotalHTPrice,
                    TotalAmountTTC = (decimal)TotalTTCPrice,
                    Tva = (decimal)Taxe,
                    Css = IsCssCheck == true ? Css : 0.0m,
                    TPS = IsTPSCheck == true ? TPS : 0.0m,
                    InsuranceCoveragePercent = SelectedAssurance?.CoveragePercent,
                    PatientPercent = SelectedAssurance != null ? (1m - SelectedAssurance.CoveragePercent) : 1m,
                    AmountPaid = (decimal)AmountPaid,
                    DiscountPercent = DiscountPercent,
                    DiscountFlat = DiscountFlat,
                    Status = StatusType.Non_payer,
                    PaymentMethod = ConvertPaymentMethodToString(PaymentMethod),

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
                FacturePdfPath = GenerateFacturePdfPath();
                DiscountFlat = 0;
                DiscountPercent = 0;
                ShowAdvanceInvoiceParam = false;
                AmountPaid = 0;
                messageBox.Show("Opération réussie",$"Création de la facture {nouvelleFacture.Reference} terminé", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                messageBox.Show("Erreur", "l'opération ne s'est pas effectuer, recommencer. Si le problème persiste, contacter l'informatitien.", MessageBoxButton.OK);
                if (ex.InnerException != null)
                    messageBox.Show("Details",$"{ex.InnerException.Message}", MessageBoxButton.OK);
            }
        }   
        [RelayCommand(CanExecute = nameof(CanExecuteCreatePatient))]
        public async Task CreatePatient()
        {
            var messageBox = new ModelOpenner();
            if (messageBox.Show("Création de patient", "Voulez vous vraiment ajouter ce patient ?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return; // L'utilisateur a annulé l'opération
            }
            Patient!.FirstName = FirstName;
            Patient.LastName = LastName;
            Patient.PhoneNumber = PhoneNumber1;
            Patient.PhoneNumber2 = PhoneNumber2;
            Patient.DateOfBirth = DateOfBirth;

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
                var MessageBox = new ModelOpenner("Utilisateur Ajouter correctement ajouter");
            }
            catch (Exception ex)
            {

                throw new System.InvalidOperationException("Erreur lors de la création du patient", ex);
            }
        }
        [RelayCommand(CanExecute = nameof(CanExecutePreviewFacture))]
        public async Task PreviewInvoiceAsync()
        {

            try
            {
                // 1. Générer et enregistrer le PDF
                // Assurez-vous que cette fonction sauvegarde le fichier PDF sur le disque.
                GenererFacturePdf();

                // 2. Lancer le processus pour ouvrir le fichier
                // Windows utilise le programme associé au type de fichier (.pdf).
                Process.Start(new ProcessStartInfo(FacturePdfPath)
                {
                    UseShellExecute = true // Crucial pour laisser Windows identifier le programme
                });

                // 3. Optionnel : Nettoyage différé
                // Si vous utilisez un fichier temporaire, vous devrez attendre que l'utilisateur ferme 
                // l'application PDF avant de supprimer le fichier. 
                // C'est pourquoi il est souvent préférable de ne pas supprimer les fichiers temporaires immédiatement.


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'ouvrir le PDF. Veuillez vérifier que Adobe Reader ou un autre lecteur PDF est installé et défini par défaut. Erreur: {ex.Message}");
            }
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
                var factureCount = await context.Factures.CountAsync() + 1;
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
            switch (DiscountType)
            {
                case "Percent":
                    NetAPayer = (decimal)(TotalHTPrice - TotalHTPrice * DiscountPercent);
                    TotalHTPrice -= TotalHTPrice * DiscountPercent;
                    break;
                case "Flat":
                    NetAPayer = (decimal)(TotalHTPrice - DiscountFlat);
                    break;
            }
            TotalTTCPrice = (double)NetAPayer;
            if (IsCssCheck)
            {
                CalculateCss = (double)(NetAPayer * Css);
                TotalTTCPrice += CalculateCss;
            }
            else
                CalculateCss = 0;
            if (IsTPSCheck)
            {
                CalculateTPS = (double)(NetAPayer * TPS);
                TotalTTCPrice += CalculateTPS;
            }
            else
                CalculateTPS = 0;
            AmountLeft = (decimal)TotalTTCPrice - (decimal)AmountPaid;
        }
        public async Task GetExamenList()
        {
            var messageBox = new ModelOpenner();
            try
            {
                using var context = new ClimaDbContext();
                var examensList = await context.Examens.ToListAsync();
                AvailableExamens.Clear();
                foreach (var e in examensList) AvailableExamens.Add(e);
            }
            catch (Exception ex)
            {

                messageBox.Show("Erreur lors du chargement des examens", $"Erreur {ex.Message}", MessageBoxButton.OK);
                if(ex.InnerException != null)
                    MessageBox.Show("Détails de l'erreur interne", $"Erreur {ex.InnerException.Message}", MessageBoxButton.OK);
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
        public void GenererFacturePdf()
        {
            var folderPath = "c:/clima-g/factures/";

            // Vérifie si le dossier n'existe PAS
            if (!Directory.Exists(folderPath))
            {
                // Crée tous les répertoires et sous-répertoires dans le chemin spécifié.
                // Cette méthode est sûre, même si le chemin contient plusieurs niveaux non existants.
                Directory.CreateDirectory(folderPath); // ✨ Le dossier est créé ici

                Console.WriteLine($"Dossier créé : {folderPath}");
            }

            // Crée le document
            var document = new FactureDocument(Facture, Patient, SessionService.User!);

            // Génère le PDF et l'ouvre
            document.GeneratePdf(FacturePdfPath);
        }
        public string GenerateFacturePdfPath()
        {
            var folderPath = "c:/clima-g/factures/";
            var fileName = $"{Facture.Reference}_{Patient?.FirstName}_{Patient?.LastName}.pdf";
            // 1. Définir le chemin complet du fichier
            string filePath = Path.Combine(folderPath, fileName);

            return filePath;
        }


        // CanExecute Methods
        private bool CanExecuteAddInvoiceExam()
        {
            // Le bouton est actif SEULEMENT si un examen est sélectionné dans le ComboBox
            return SelectedAvailableExam != null;
        }
        private bool CanExecuteCreateFact()
        {
            return InvoiceExams.Count > 0;
        }
        private bool CanExecutePreviewFacture()
        {
            return FacturePdfPath != string.Empty;
        }
        private bool CanExecuteCreatePatient()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber1) && DateOfBirth is not null;
        }

        public string ConvertPaymentMethodToString(PaymentMethod paymentMethod)
        {
            switch (paymentMethod)
            {
                case PaymentMethod.Especes: // CORRIGÉ : On teste la valeur directement
                    return "Espèces";

                case PaymentMethod.Cheque: // AJOUTÉ : Deuxième cas
                    return "Cheque";

                case PaymentMethod.MobileMoney: // AJOUTÉ : Troisième cas
                    return "Mobile Money";

                default: // AJOUTÉ : Pour gérer les cas imprévus ou futurs
                    return "Non spécifié";
            }
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
