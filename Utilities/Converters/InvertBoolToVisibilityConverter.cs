using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace invoice.Utilities.Converters
{
    public class InvertBoolToVisibilityConverter : IValueConverter
    {
        // Cette méthode est appelée lors du Binding (ViewModel -> View/XAML)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                // Si la valeur est TRUE, le contrôle est Visible
                if (isVisible)
                {
                    return Visibility.Collapsed;
                }

                // Si FALSE, le contrôle est afficher
                return Visibility.Visible;
            }

            // Valeur par défaut si la conversion échoue
            return Visibility.Collapsed;
        }

        // Cette méthode est rarement utilisée dans ce contexte (View -> ViewModel)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Collapsed;
        }
    }
}
