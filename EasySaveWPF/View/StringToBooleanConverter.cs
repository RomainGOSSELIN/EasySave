using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EasySaveWPF.View
{
    public class StringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Vérifie si la valeur passée correspond au paramètre de conversion
            if (value?.ToString() == parameter?.ToString())
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
                return parameter?.ToString();
            }
            return null; // ou une valeur par défaut si nécessaire
        }
    }
}
