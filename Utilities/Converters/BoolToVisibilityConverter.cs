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
    public class BoolToVisibilityConverter : IValueConverter
    {
        // Cette méthode est appelée lors du Binding (ViewModel -> View/XAML)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                // Si la valeur est TRUE, le contrôle est Visible
                if (isVisible)
                {
                    return Visibility.Visible;
                }

                // Gérer le paramètre "inversé" pour masquer le PasswordBox
                if (parameter?.ToString() == "invert")
                {
                    return Visibility.Visible;
                }

                // Si FALSE, le contrôle est masqué (Collapsed est préféré car il ne prend pas d'espace)
                return Visibility.Collapsed;
            }

            // Valeur par défaut si la conversion échoue
            return Visibility.Collapsed;
        }

        // Cette méthode est rarement utilisée dans ce contexte (View -> ViewModel)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Retourne true si Visible, false sinon
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }
}
