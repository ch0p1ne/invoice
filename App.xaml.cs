using invoice.Services;
using invoice.ViewModels;
using invoice.Views;
using QuestPDF.Infrastructure;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace invoice
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        //Constructor
        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            // Variable de session disponible et partager entre toutes les VM
            var sessionService = new SessionService();

            var navigationService = new NavigationService(sessionService);
            navigationService.NavigateTo<LoginVM>();

        }
        protected override void OnStartup(StartupEventArgs e)
        {

            QuestPDF.Settings.License = LicenseType.Community;
            // Alternative plus récente :
            
            // Le code de culture pour le français (Gabon)
            string gabonCultureCode = "fr-GA";

            CultureInfo culture = new CultureInfo(gabonCultureCode);

            // 1. Définir la culture pour la mise en forme (nombres, devises, dates)
            Thread.CurrentThread.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fr-GA");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fr-GA");

            base.OnStartup(e);
        }


    }

}
