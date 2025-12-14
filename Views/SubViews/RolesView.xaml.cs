using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using invoice.CustomControls;
using invoice.ViewModels.SubViewModels;

namespace invoice.Views.SubViews
{
    /// <summary>
    /// Logique d'interaction pour RoleView.xaml
    /// </summary>
    public partial class RolesView : UserControl
    {
        public RolesView()
        {
            InitializeComponent();
            DataContext = new RolesVM();
            // Écoute de l'événement RequestClose provenant du AddRoleForm (bubbling)
            OverlayContent.AddHandler(AddRoleForm.RequestCloseEvent, new RoutedEventHandler(OnAddRoleFormRequestClose));
        }

        private void ShowFormButton_Click(object sender, RoutedEventArgs e)
        {
            OverlayHost.Visibility = Visibility.Visible;
            OverlayHost.IsHitTestVisible = true;
            OverlayContent.Content = this.Resources["AddRoleForm"] as AddRoleForm;

            // Optionnel : lancer un storyboard d'entrée si présent
            if (OverlayContent.Content is AddRoleForm form)
            {
                if (form.TryFindResource("EnterStoryboard") is Storyboard enterSb)
                {
                    enterSb.Begin(form);
                }
            }
        }

        private void OnAddRoleFormRequestClose(object? sender, RoutedEventArgs e)
        {
            // Récupérer l'instance réelle du contrôle ajouté
            if (OverlayContent.Content is AddRoleForm AddRoleForm)
            {
                // Chercher le Storyboard nommé ExitStoryboard dans les ressources du contrôle
                if (AddRoleForm.TryFindResource("ExitStoryboard") is Storyboard sb)
                {
                    // Lorsque l'animation est terminée, masquer l'overlay et nettoyer si besoin
                    void OnCompleted(object s, EventArgs args)
                    {
                        sb.Completed -= OnCompleted;
                        OverlayHost.Visibility = Visibility.Collapsed;
                        OverlayContent.Content = null;
                        OverlayHost.IsHitTestVisible = false;
                    }

                    sb.Completed += OnCompleted;

                    // Lancer l'animation en ciblant le contrôle
                    sb.Begin(AddRoleForm);
                    return;
                }

                // Fallback : si pas d'animation, masquer immédiatement
                OverlayHost.Visibility = Visibility.Collapsed;
                OverlayContent.Content = null;
                OverlayHost.IsHitTestVisible = false;
            }
            else
            {
                // Aucun contrôle trouvé → masquer par sécurité
                OverlayHost.Visibility = Visibility.Collapsed;
            }
        }

    }
}
