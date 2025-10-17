using invoice.Services;
using invoice.ViewModels;
using invoice.Views;
using System.Configuration;
using System.Data;
using System.Windows;

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
            navigationService.NavigateTo<MainVM>();


        }
    }

}
