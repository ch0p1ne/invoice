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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace invoice.Views
{
    /// <summary>
    /// Logique d'interaction pour UserAndRoleView.xaml
    /// </summary>
    public partial class UserAndRoleView : UserControl
    {
        /*
         Plan (pseudocode détaillé) :
         - Définir une DependencyProperty `CurrentIndexProperty` de type int, avec BindsTwoWayByDefault.
         - Ajouter l'accesseur CLR `CurrentIndex` qui utilise GetValue/SetValue.
         - Fournir un callback statique `OnCurrentIndexChanged` qui appelle une méthode d'instance `OnCurrentIndexChanged`.
         - Dans `OnCurrentIndexChanged(int newIndex)` :
             - Calculer la position cible du pill (newIndex * SegmentButtonWidth).
             - Lancer l'animation du `PillTransform` vers cette position.
             - Mettre à jour les couleurs des boutons en sélectionnant celui dont le Tag correspond à newIndex.
         - Modifier `SegmentButton_Click` pour affecter `CurrentIndex = targetIndex` (la logique d'animation sera centralisée dans le callback).
         - Supprimer le champ privé `_currentIndex`.
         - Garder la propriété bindable pour être facilement liée au ViewModel courant en TwoWay.
        */

        private const double AnimmationDuration = 0.35;
        private const double SegmentButtonWidth = 120;

        // DependencyProperty pour lier l'index courant avec le ViewModel (TwoWay par défaut)
        public static readonly DependencyProperty CurrentIndexProperty = DependencyProperty.Register(
            nameof(CurrentIndex),
            typeof(int),
            typeof(UserAndRoleView),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCurrentIndexChanged));

        public int CurrentIndex
        {
            get => (int)GetValue(CurrentIndexProperty);
            set => SetValue(CurrentIndexProperty, value);
        }

        private static void OnCurrentIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UserAndRoleView view && e.NewValue is int newIndex)
            {
                view.OnCurrentIndexChanged(newIndex);
            }
        }

        private void OnCurrentIndexChanged(int newIndex)
        {
            try
            {
                double targetOffset = newIndex * SegmentButtonWidth;

                if (PillTransform != null)
                {
                    // Si la position est déjà correcte, on ne relance pas l'animation
                    if (Math.Abs(PillTransform.X - targetOffset) > 0.001)
                    {
                        DoubleAnimation animation = new()
                        {
                            To = targetOffset,
                            Duration = TimeSpan.FromSeconds(AnimmationDuration),
                            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
                        };

                        PillTransform.BeginAnimation(TranslateTransform.XProperty, animation);
                    }
                }

                // Mettre à jour les couleurs des boutons en fonction de l'index
                foreach (var child in SegmentGrid.Children)
                {
                    if (child is Button button)
                    {
                        button.Foreground = new SolidColorBrush(Color.FromArgb(255, 153, 153, 153));

                        if (int.TryParse(button.Tag?.ToString(), out int idx) && idx == newIndex)
                        {
                            button.Foreground = Brushes.White;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occur during OnCurrentIndexChanged: {ex.Message}");
            }
        }

        public UserAndRoleView()
        {
            InitializeComponent();
            Console.WriteLine("Segmented control initialized");
        }

        private void SegmentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button clickedButton)
                {
                    if (int.TryParse(clickedButton.Tag?.ToString(), out int targetIndex))
                    {
                        // Utiliser la DependencyProperty pour propager la valeur au ViewModel et déclencher le callback d'UI
                        if (CurrentIndex == targetIndex)
                        {
                            return; // pas de changement
                        }

                        CurrentIndex = targetIndex;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occur during segment click: {ex.Message}");
            }
        }

        private void UpdateTextColors(Button clickedButton)
        {
            foreach (var child in SegmentGrid.Children)
            {
                if (child is Button button)
                {
                    button.Foreground = new SolidColorBrush(Color.FromArgb(255, 153, 153, 153));
                }
            }

            clickedButton.Foreground = Brushes.White;
        }
    }
}
