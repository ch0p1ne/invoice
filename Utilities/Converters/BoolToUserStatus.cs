using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace invoice.Utilities.Converters
{
    public class BoolToUserStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool status)
            {
                switch(status)
                {
                    case true:
                        return "Actif";
                    case false:
                        return "Inactif";
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string status)
            {
                switch (status)
                {
                    case "Actif":
                        return true;
                    case "Inactif":
                        return false;
                }
            }
            return false;
        }
    }
}
