using invoice.Utilities;
using invoice.ViewModels;
using invoice.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace invoice.Services
{
    public class NavigationService : INavigationService
    {
        // Mapping d'un viewModel a sa View (Window) correspondante.
        private readonly Dictionary<Type, Type> _mappingViewAndViewModel = new()
        {
            { typeof(LoginVM), typeof(LoginWindow) },
            { typeof(MainVM), typeof(MainWindow) }
        };

        // Mapping d'un viewModel au constructeur voulue.
        private readonly Dictionary<Type, Func<object>> _factories = new();

        
        public NavigationService(ISessionService sessionService)
        {
            _factories[typeof(LoginVM)] = () => new LoginVM(this, sessionService);
            _factories[typeof(MainVM)] = () => new MainVM(this, sessionService);
        }

        public void CloseWindow<TViewModel>() where TViewModel : VMBase
        {
            var view = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext?.GetType()  == typeof(TViewModel));

            view?.Close();
        }

        public void NavigateTo<TViewModel>() where TViewModel : VMBase
        {
            var vmType = typeof(TViewModel);

            if (!_mappingViewAndViewModel.TryGetValue(vmType, out Type? viewType))
            {
                throw new InvalidOperationException($"Aucune view mappé pour {vmType.Name}");
            }

            // creation de l'instance de la viewModel : TViewModel.
            var viewModel = _factories[vmType]();
            if(viewModel is MainVM mainVM)
            {
                _ = mainVM.InitializeAsync();
            }

            // Crée la view correspondante.
            var view = Activator.CreateInstance(viewType) as Window ?? throw new InvalidOperationException($"Erreur lors de l'instanciation de {viewType.Name} associer a la vm : {vmType}");

            view.DataContext = viewModel;
            view.Show();

        }
    }
}
