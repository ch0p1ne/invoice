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
        private const double AnimmationDuration = 0.35;
        private const double SegmentButtonWidth = 120;

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
                        double targetOffset = targetIndex * SegmentButtonWidth;

                        if(PillTransform.X == targetOffset)
                        {
                            return; // No need to animate if already in position
                        }

                        DoubleAnimation animation = new()
                        {
                            To = targetOffset,
                            Duration = TimeSpan.FromSeconds(AnimmationDuration),
                            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
                        };

                        PillTransform.BeginAnimation(TranslateTransform.XProperty, animation);

                        UpdateTextColors(clickedButton);

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
