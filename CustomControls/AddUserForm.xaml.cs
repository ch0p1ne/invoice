using invoice.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace invoice.CustomControls
{
    /// <summary>
    /// Logique d'interaction pour AddUserForm.xaml
    /// </summary>
    public partial class AddUserForm : UserControl
    {
        // Routed event pour demander la fermeture (bubbling)
        public static readonly RoutedEvent RequestCloseEvent =
            EventManager.RegisterRoutedEvent(
                nameof(RequestClose),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(AddUserForm));

        public event RoutedEventHandler RequestClose
        {
            add => AddHandler(RequestCloseEvent, value);
            remove => RemoveHandler(RequestCloseEvent, value);
        }

        public AddUserForm()
        {
            InitializeComponent();
            Loaded += AddUserForm_Loaded;
            DataContext = new UsersVM();
        }

        private void AddUserForm_Loaded(object sender, RoutedEventArgs e)
        {
            // Cherche un bouton dont le contenu est "Annuler" et attache le handler
            // Cela évite d'éditer le XAML si le nom du bouton est inconnu
            AttachCancelHandler(this);
        }

        private void AttachCancelHandler(DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is Button btn)
                {
                    if (btn.Content is string s && s.Trim().Equals("Annuler", System.StringComparison.OrdinalIgnoreCase))
                    {
                        btn.Click += CancelButton_Click;
                        return;
                    }
                }

                AttachCancelHandler(child);
            }
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            RaiseRequestClose();
        }

        // Méthode publique que le bouton Cancel dans le XAML doit appeler
        public void RaiseRequestClose()
        {
            RaiseEvent(new RoutedEventArgs(RequestCloseEvent, this));
        }
    }
}
