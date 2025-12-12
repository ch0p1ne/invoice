using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using invoice.CustomControls;

namespace invoice.Views.SubViews
{
    /// <summary>
    /// Logique d'interaction pour UserView.xaml
    /// </summary>
    public partial class UsersView : UserControl
    {
        public UsersView()
        {
            InitializeComponent();

            // Écoute de l'événement RequestClose provenant du AddUserForm (bubbling)
            OverlayContent.AddHandler(AddUserForm.RequestCloseEvent, new RoutedEventHandler(OnAddUserFormRequestClose));
        }

        private void ShowFormButton_Click(object sender, RoutedEventArgs e)
        {
            OverlayHost.Visibility = Visibility.Visible;
            OverlayHost.IsHitTestVisible = true;
            OverlayContent.Content = this.Resources["AddUserForm"] as AddUserForm;

            // Optionnel : lancer un storyboard d'entrée si présent
            if (OverlayContent.Content is AddUserForm form)
            {
                if (form.TryFindResource("EnterStoryboard") is Storyboard enterSb)
                {
                    enterSb.Begin(form);
                }
            }
        }

        private void OnAddUserFormRequestClose(object? sender, RoutedEventArgs e)
        {
            // Récupérer l'instance réelle du contrôle ajouté
            if (OverlayContent.Content is AddUserForm addUserForm)
            {
                // Chercher le Storyboard nommé ExitStoryboard dans les ressources du contrôle
                if (addUserForm.TryFindResource("ExitStoryboard") is Storyboard sb)
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
                    sb.Begin(addUserForm);
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
