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
    public class DiscountFormatConverter : IMultiValueConverter
    {
        // Convert : Prend toutes les valeurs du MultiBinding dans 'values'
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // 1. VÉRIFIEZ LES VALEURS
            // Le premier élément (values[0]) est la valeur numérique (DiscountAmount)
            // Le second élément (values[1]) est l'état du RadioButton (IsChecked)

            if (values.Length < 2 || values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
            {
                return string.Empty;
            }

            object numericValue = values[0];
            bool? isPercentageChecked = values[1] as bool?; // Le RadioButton.IsChecked est un bool?

            // Gérer le cas où la valeur numérique est null
            if (numericValue == null)
            {
                return string.Empty;
            }

            // 2. LOGIQUE DE FORMATAGE
            if (isPercentageChecked.GetValueOrDefault(false)) // Si le RadioButton pourcentage est coché
            {
                // Format pour le pourcentage (e.g., "15 %")
                string formatString = "{0:P0}";
                return string.Format(culture, formatString, numericValue);
            }
            else
            {
                // Format pour le montant fixe (monétaire)
                string formatString = "{0:N0} Fcfa";
                return string.Format(culture, formatString, numericValue);
            }
        }

        // ConvertBack : Obligatoire pour l'interface, mais souvent non implémenté pour l'affichage
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return [];
        }
    }
}
