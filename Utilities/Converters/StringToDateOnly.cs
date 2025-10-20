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
    public class StringToDateOnly : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly dateOnly)
            {
                // Convertit DateOnly en un DateTime non-nullable (avec l'heure à minuit)
                return dateOnly.ToDateTime(TimeOnly.MinValue);
            }

            // Si la DateOnly est null ou non initialisée, retourne null pour désélectionner
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime ? dateTimeValue = value as DateTime?;

            if (dateTimeValue.HasValue)
            {
                // 2. Si la valeur existe, utilise FromDateTime pour obtenir la DateOnly
                // (Ceci ignorera la partie heure du DateTime)
                return DateOnly.FromDateTime(dateTimeValue.Value);
            }

            // 3. Si la date est effacée ou non définie (null), on retourne la valeur par défaut pour DateOnly
            // Vous pouvez retourner 'DateOnly.MinValue' ou une autre valeur par défaut pour votre entité.
            return default(DateOnly);

        }
    }
}
