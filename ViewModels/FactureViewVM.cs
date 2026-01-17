using CommunityToolkit.Mvvm.Input;
using HelperPagination.ViewModels;
using invoice.Context;
using invoice.Models;
using invoice.Services;
using invoice.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Process = System.Diagnostics.Process;

namespace invoice.ViewModels
{
    public partial class FactureViewVM : VMBase
    {
        private const int CONST_ITEMS_PER_PAGE = 35;
        // Fields
        private string _searchTerm = string.Empty;
        private Facture? _selectedFacture;
        private bool _isFactureIsSelected = false;
        private decimal _amountLeft = decimal.Zero;

        private bool estChargement = false;
        private bool estRechercher = false;
        private bool _showdetailInvoice = false;
        private string _showdetailButtonText = "O";
        private StatusType _estChargementStatus;
        public PaginationVM PaginationVM { get; set; } = new PaginationVM(CONST_ITEMS_PER_PAGE, 0);
        public StatusType EstChargementStatus
        {
            get => _estChargementStatus;
            set
            {
                if (_estChargementStatus != value)
                {
                    _estChargementStatus = value;
                    PaginationVM.UpdateCurrentPage(1);
                }
            }
        }
        // Properties
        public decimal AmountLeft
        {
            get => _amountLeft;
            set => SetProperty(ref _amountLeft, value);
        }
        public string SearchTerm
        {
            get => _searchTerm;
            set => SetProperty(ref _searchTerm, value);
        }
        public bool ShowDetailInvoice
        {
            get => _showdetailInvoice;
            set => SetProperty(ref _showdetailInvoice, value);
        }
        public string ShowDetailButtonText
        {
            get => _showdetailButtonText;
            set => SetProperty(ref _showdetailButtonText, value);
        }
        public Facture? SelectedFacture
        {
            get => _selectedFacture;
            set
            {
                SetProperty(ref _selectedFacture, value);
                IsFactureIsSelected = value != null;
                MarquerNonPayerCommand.NotifyCanExecuteChanged();
                MarquerPayerCommand.NotifyCanExecuteChanged();
                DeleteFactureCommand.NotifyCanExecuteChanged();
                PrintInvoiceDirectlyCommand.NotifyCanExecuteChanged();
                PreviewInvoiceCommand.NotifyCanExecuteChanged();
                CalculateAmountLeft();
            }
        }
        public bool IsFactureIsSelected
        {
            get => _isFactureIsSelected;
            set
            {
                SetProperty(ref _isFactureIsSelected, value);
                PrintInvoiceDirectlyCommand.NotifyCanExecuteChanged();
            }
        }
        public ObservableCollection<Facture> Factures { get; set; } = new ObservableCollection<Facture>();

        // Constructor
        public FactureViewVM()
        {
            PaginationVM.PageChanged += OnPageChanged;
            RetrieveInvoiceTotalItems();
            PaginationVM.UpdateTotalPages();
            PaginationVM.UpdateCurrentPage(1);
            LoadLastFacture().ConfigureAwait(false);
        }

        // Methods
        [RelayCommand]
        public async Task LoadLastFacture()
        {
            try
            {

                estChargement = false;
                estRechercher = false;
                RetrieveInvoiceTotalItems();
                PaginationVM.UpdateTotalPages();
                using var db = new ClimaDbContext();

                // Construire la requête optimisée
                var query = db.Factures
                    .AsNoTracking()      // Pas de suivi -> moins d'overhead mémoire
                    .AsSplitQuery()      // Évite les joins massifs / explosion cartésienne
                    .Include(f => f.Patient)
                    .Include(f => f.FacturesExamens)
                        .ThenInclude(fe => fe.Examen)
                    // Inclure les deux navigations de FacturesConsultations si nécessaire.
                    // Les appels séparés à Include + ThenInclude sont acceptables et
                    // restent efficaces avec AsSplitQuery.
                    .Include(f => f.FacturesConsultations)
                        .ThenInclude(fc => fc.Consultation)
                    .Include(f => f.FacturesConsultations)
                        .ThenInclude(fc => fc.Medecin)
                    .Include(f => f.User)
                    .OrderByDescending(f => f.Created_at)
                    .Skip((PaginationVM.Pagination.Current_page - 1) * PaginationVM.Pagination.Items_per_page)
                    .Take(PaginationVM.Pagination.Items_per_page);

                var factures = await query.ToListAsync();

                Factures.Clear();
                foreach (var facture in factures)
                {
                    Factures.Add(facture);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($" Erreur survenue pendant le chargement de toutes les factures : {ex.Message}");
            }
        }
        private void RetrieveInvoiceTotalItems()
        {
            try
            {
                using var context = new ClimaDbContext();
                int totalItems = context.Factures.Count();

                PaginationVM.UpdateTotalItems(totalItems);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération du nombre total d'éléments : {ex.Message}");
            }
        }
        private void RetrieveInvoiceTotalItemsFiltered(StatusType status)
        {
            try
            {
                using var context = new ClimaDbContext();
                int totalItems = context.Factures.Count(f => f.Status == status);
                PaginationVM.UpdateTotalItems(totalItems);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération du nombre total d'éléments : {ex.Message}");
            }
        }
        private void RetrieveInvoiceTotalItemsSearch(string searchTerm)
        {
            try
            {
                using var context = new ClimaDbContext();
                int totalItems = context.Factures
                    .Count(f => f.Patient!.LastName.Contains(searchTerm) || f.Patient!.FirstName.Contains(searchTerm) || f.Reference.Contains(searchTerm));
                PaginationVM.UpdateTotalItems(totalItems);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération du nombre total d'éléments : {ex.Message}");
            }
        }
        private void CalculateAmountLeft()
        {
            if (SelectedFacture != null && SelectedFacture.AmountPaid == 0m)
            {
                AmountLeft = 0m;
                return;
            }

            AmountLeft = SelectedFacture != null ? (decimal)(SelectedFacture.TotalAmountTTC - SelectedFacture.AmountPaid!) : 0m;
        }

        public string GenererFacturePdf()
        {
            // --- 1. DÉFINITION DU CHEMIN ET DU NOM DE FICHIER ---

            // 💡 Bonne pratique : Utiliser Path.Combine pour construire le chemin du dossier
            // Remplacer le chemin absolu codé en dur par une variable si possible (ex: AppDomain.CurrentDomain.BaseDirectory)
            var folderPath = "c:\\clima-g\\factures"; // Utiliser des doubles anti-slash ou le @ littéral

            // Le nom du fichier
            // On utilise Path.GetInvalidFileNameChars pour retirer les caractères invalides 
            // des noms/prénoms (même si c'est rare, c'est une sécurité)
            string cleanFirstName = string.Join("_", SelectedFacture!.Patient!.FirstName.Split(Path.GetInvalidFileNameChars()));
            string cleanLastName = string.Join("_", SelectedFacture.Patient!.LastName.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"{SelectedFacture.Reference}_{cleanFirstName}_{cleanLastName}.pdf";

            // Le chemin complet
            string filePath = Path.Combine(folderPath, fileName);

            // --- 2. VÉRIFICATION DE L'EXISTENCE DU FICHIER ---

            if (File.Exists(filePath))
            {
                // Si le fichier existe déjà, on retourne immédiatement le chemin
                Console.WriteLine($"Facture déjà générée, chemin retourné : {filePath}");
                return filePath;
            }

            // --- 3. PRÉPARATION DU DOSSIER ET GÉNÉRATION (Si le fichier n'existe PAS) ---

            // Vérifie si le dossier n'existe PAS et le crée si nécessaire.
            if (!Directory.Exists(folderPath))
            {
                // Crée tous les répertoires et sous-répertoires dans le chemin spécifié.
                Directory.CreateDirectory(folderPath);
                Console.WriteLine($"Dossier créé : {folderPath}");
            }

            try
            {
                // Crée le document (Utilisation de l'objet de génération de PDF)
                switch (SelectedFacture.Type)
                {
                    case InvoiceType.Examen:
                        var document = new FactureDocument(SelectedFacture, SelectedFacture.Patient, SelectedFacture.User!);
                        document.GeneratePdf(filePath);
                        break;
                    case InvoiceType.Consultation:
                        Medecin? medecin = null;
                        using (var context = new ClimaDbContext())
                        {
                            var factureId = SelectedFacture.FactureId;
                            if (factureId != 0)
                            {
                                // Récupère le premier MedecinId associé aux lignes de consultation de la facture
                                var medecinId = context.FacturesConsultations
                                    .Where(fc => fc.FactureId == factureId)
                                    .Select(fc => fc.MedecinId)
                                    .FirstOrDefault();

                                if (medecinId != 0)
                                {
                                    medecin = context.Medecins.Find(medecinId);
                                }
                            }
                        }

                        // Si aucun medecin trouvé, on passe une instance vide (ou gérer autrement selon vos besoins)
                        medecin ??= new Medecin();

                        var document2 = new FactureDocument(SelectedFacture, SelectedFacture.Patient, medecin, SelectedFacture.User!);
                        document2.GeneratePdf(filePath);
                        break;
                }
                Console.WriteLine($"Facture générée avec succès : {filePath}");
            }
            catch (Exception ex)
            {
                // Gestion des erreurs de génération/écriture
                Console.WriteLine($"Erreur lors de la génération du PDF : {ex.Message}");
                // Si la génération échoue, on peut retourner un chemin vide ou lever l'exception
                return string.Empty;
            }

            // Retourne le chemin (que le fichier ait été trouvé ou généré)
            return filePath;
        }



        // Commands
        [RelayCommand(CanExecute = nameof(CanPreviewFacture))]
        public async Task PreviewInvoice()
        {
            // 1. Générer et enregistrer le PDF
            // Assurez-vous que cette fonction sauvegarde le fichier PDF sur le disque.
            string pdfPath = GenererFacturePdf();

            if (!File.Exists(pdfPath))
            {
                MessageBox.Show("Erreur : Le fichier PDF n'a pas été trouvé.");
                return;
            }

            try
            {
                // 2. Lancer le processus pour ouvrir le fichier
                // Windows utilise le programme associé au type de fichier (.pdf).
                Process.Start(new ProcessStartInfo(pdfPath)
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
        [RelayCommand]
        public async Task RefreshFactures()
        {
            RetrieveInvoiceTotalItems();
            PaginationVM.UpdateTotalPages();
            PaginationVM.UpdateCurrentPage(1);
            OnPageChanged(this, EventArgs.Empty);

        }
        [RelayCommand]
        public async Task GoToPage()
        {
            RetrieveInvoiceTotalItems();
            PaginationVM.UpdateTotalPages();
            OnPageChanged(this, EventArgs.Empty);

        }
        [RelayCommand]
        public async Task FilterItemsPerPage()
        {
            //RetrieveInvoiceTotalItems(); pas necessaire pour l'opération (atomiquement parlant)
            PaginationVM.UpdateTotalPages();
            OnPageChanged(this, EventArgs.Empty);

        }
        [RelayCommand(CanExecute = nameof(CanMarquerPayer))]
        public async Task MarquerPayer()
        {
            var modelOpenner = new ModelOpenner();
            var result = modelOpenner.Show("Marquer comme payée", "Voulez-vous vraiment marquer cette facture comme payée ?", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    using (var db = new ClimaDbContext())
                    {
                        var factureToUpdate = db.Factures.FirstOrDefault(f => f.FactureId == SelectedFacture!.FactureId);
                        if (factureToUpdate != null)
                        {
                            factureToUpdate.Status = StatusType.Payer;
                            await db.SaveChangesAsync();
                            SelectedFacture!.Status = StatusType.Payer;
                            await RefreshFactures();
                            var MessageBox = new ModelOpenner("Status de la facture mise à jour");

                        }
                    }
                    break;
                case MessageBoxResult.No:
                    // Do nothing
                    break;
            }
        }
        [RelayCommand(CanExecute = nameof(CanPrintFacture))]
        public void PrintInvoiceDirectly()
        {
            // 1. Choisir l'imprimante (via la boîte de dialogue WPF/Windows.Forms)
            var printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() != true) return;

            string printerName = printDialog.PrintQueue.FullName;
            string pdfPath = GenererFacturePdf(); // Le chemin de votre PDF

            // 2. Tenter de trouver le chemin de l'exécutable Adobe Reader (très courant)
            // NOTE: Vous pourriez rendre ce chemin configurable si vous supportez plusieurs lecteurs (Foxit, Edge, etc.)
            string adobePath = @"C:\Program Files\Adobe\Acrobat DC\Acrobat\Acrobat.exe";

            if (!File.Exists(adobePath))
            {
                MessageBox.Show("Erreur : Le programme Adobe Reader n'a pas été trouvé. L'impression directe est annulée.");
                // Vous pouvez tenter la méthode ShellExecute ici comme solution de secours.
                return;
            }

            try
            {
                // 3. Construction des arguments pour l'impression silencieuse :
                // /t <chemin_du_pdf> <nom_de_l'imprimante>
                string arguments = $"/t \"{pdfPath}\" \"{printerName}\"";

                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = adobePath,         // 💡 L'exécutable lui-même
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false       // On ne passe plus par le shell
                };

                Process.Start(info);
                Thread.Sleep(2000); // Attendre 5 secondes pour s'assurer que l'impression démarre
                MessageBox.Show("Commande d'impression envoyée avec succès.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'impression directe : {ex.Message}");
            }
            finally
            {
            }
        }
        [RelayCommand(CanExecute = nameof(CanMarquerNonPayer))]
        public async Task MarquerNonPayer()
        {
            var modelOpenner = new ModelOpenner();
            var result = modelOpenner.Show("Marquer comme non payée", "Voulez-vous vraiment marquer cette facture comme non payée ?", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    using (var db = new ClimaDbContext())
                    {
                        var factureToUpdate = db.Factures.FirstOrDefault(f => f.FactureId == SelectedFacture!.FactureId);
                        if (factureToUpdate != null)
                        {
                            factureToUpdate.Status = StatusType.Non_payer;
                            await db.SaveChangesAsync();
                            SelectedFacture!.Status = StatusType.Non_payer;
                            await RefreshFactures();
                            var MessageBox = new ModelOpenner("Status de la facture mise à jour");
                        }
                    }
                    break;
                case MessageBoxResult.No:
                    // Do nothing
                    break;
            }
        }
        [RelayCommand(CanExecute = nameof(CanDeleteFacture))]
        public async Task DeleteFacture()
        {
            var modelOpenner = new ModelOpenner();
            var result = modelOpenner.Show("Suppression de la facture", "Voulez-vous vraiment supprimer cette facture ?", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    using (var db = new ClimaDbContext())
                    {
                        var factureToDelete = db.Factures.FirstOrDefault(f => f.FactureId == SelectedFacture.FactureId);
                        if (factureToDelete != null)
                        {
                            db.Factures.Remove(factureToDelete);
                            await db.SaveChangesAsync();
                            Factures.Remove(SelectedFacture!);
                            SelectedFacture = null;
                            var MessageBox = new ModelOpenner("Facture supprimée avec succès");
                            await RefreshFactures();
                        }
                    }
                    break;
                case MessageBoxResult.No:
                    // Do nothing
                    break;
            }
        }
        [RelayCommand]
        public async Task SearchFactures(KeyEventArgs args)
        {
            if (args.Key != Key.Enter) return;

            var messageBox = new ModelOpenner();
            RetrieveInvoiceTotalItemsSearch(SearchTerm);
            PaginationVM.UpdateTotalPages();

            if (!estRechercher)
            {
                PaginationVM.UpdateCurrentPage(1);
                messageBox.Show("Résultat", $"{PaginationVM.Pagination.Total_items} correspondance trouvé.", MessageBoxButton.OK);
            }

            try
            {

                using (var db = new ClimaDbContext())
                {
                    var query = db.Factures
                    .AsNoTracking()      // Pas de suivi -> moins d'overhead mémoire
                    .AsSplitQuery()      // Évite les joins massifs / explosion cartésienne
                    .Include(f => f.Patient)
                    .Include(f => f.FacturesExamens)
                        .ThenInclude(fe => fe.Examen)
                    // Inclure les deux navigations de FacturesConsultations si nécessaire.
                    // Les appels séparés à Include + ThenInclude sont acceptables et
                    // restent efficaces avec AsSplitQuery.
                    .Include(f => f.FacturesConsultations)
                        .ThenInclude(fc => fc.Consultation)
                    .Include(f => f.FacturesConsultations)
                        .ThenInclude(fc => fc.Medecin)
                    .Include(f => f.User)
                    .Where(f => f.Patient!.LastName.Contains(SearchTerm) || f.Patient!.FirstName.Contains(SearchTerm) || f.Reference.Contains(SearchTerm))
                    .OrderByDescending(f => f.Created_at)
                    .Skip((PaginationVM.Pagination.Current_page - 1) * PaginationVM.Pagination.Items_per_page)
                        .Take(PaginationVM.Pagination.Items_per_page);


                    var factures = await query.ToListAsync();
                    Factures.Clear();
                    foreach (var facture in factures)
                    {
                        Factures.Add(facture);
                    }
                    args.Handled = true;
                }

                estRechercher = true;
                estChargement = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($" Erreur survenue pendant la recherche des factures : {ex.Message}");
            }
        }
        [RelayCommand]
        public async Task FilterStatusFactures(StatusType status)
        {
            // si le filtre change, on reset la pagination aussi, voir la prorpriété EstChargementStatus
            if (!estChargement)
                PaginationVM.UpdateCurrentPage(1);

            try
            {
                RetrieveInvoiceTotalItemsFiltered(status);
                PaginationVM.UpdateTotalPages();

                using var db = new ClimaDbContext();
                var query = db.Factures
                .AsNoTracking()      // Pas de suivi -> moins d'overhead mémoire
                .AsSplitQuery()      // Évite les joins massifs / explosion cartésienne
                .Include(f => f.Patient)
                .Include(f => f.FacturesExamens)
                    .ThenInclude(fe => fe.Examen)
                // Inclure les deux navigations de FacturesConsultations si nécessaire.
                // Les appels séparés à Include + ThenInclude sont acceptables et
                // restent efficaces avec AsSplitQuery.
                .Include(f => f.FacturesConsultations)
                    .ThenInclude(fc => fc.Consultation)
                .Include(f => f.FacturesConsultations)
                    .ThenInclude(fc => fc.Medecin)
                .Include(f => f.User)
                .Where(f => f.Status == status)
                .OrderByDescending(f => f.Created_at)
                .Skip((PaginationVM.Pagination.Current_page - 1) * PaginationVM.Pagination.Items_per_page)
                    .Take(PaginationVM.Pagination.Items_per_page);

                var factures = await query.ToListAsync();
                Factures.Clear();
                foreach (var facture in factures)
                {
                    Factures.Add(facture);
                }
                estChargement = true;
                estRechercher = false;
                EstChargementStatus = status;
            }
            catch (Exception)
            {

                System.Diagnostics.Debug.WriteLine($" Erreur survenue pendant le chargement des factures filtrées par status : {status} ");
            }
        }
        [RelayCommand]
        public void ToogleDetailInvoice()
        {
            IsFactureIsSelected = !IsFactureIsSelected;
            ShowDetailInvoice = !ShowDetailInvoice;
            if (ShowDetailInvoice)
            {
                ShowDetailButtonText = "X";
            }
            else
            {
                ShowDetailButtonText = "O";
            }
        }


        // Can execute methods
        private bool CanMarquerPayer()
        {
            return IsFactureIsSelected && SelectedFacture.Status != StatusType.Payer;
        }
        private bool CanMarquerNonPayer()
        {
            return IsFactureIsSelected && SelectedFacture.Status != StatusType.Non_payer;
        }
        private bool CanDeleteFacture()
        {
            return IsFactureIsSelected;
        }
        private bool CanPrintFacture()
        {
            return IsFactureIsSelected;
        }
        private bool CanPreviewFacture()
        {
            return IsFactureIsSelected;
        }

        // event 
        private async void OnPageChanged(object? sender, EventArgs e)
        {
            try
            {
                if (!estChargement && !estRechercher)
                {
                    await LoadLastFacture();
                }
                else if (estChargement && !estRechercher)
                {
                    await FilterStatusFactures(EstChargementStatus);
                }
                else if (estRechercher && !estChargement)
                {
                    await SearchFactures(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(System.Windows.Application.Current.MainWindow)!, 0, Key.Enter)
                    {
                        RoutedEvent = Keyboard.KeyDownEvent 
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du changement de page : {ex.Message}");
                throw;
            }
        }
    }
}
