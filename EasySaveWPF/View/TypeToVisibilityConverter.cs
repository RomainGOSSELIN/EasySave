using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace EasySaveWPF.View
{
    public class TypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Déterminez ici la logique pour convertir la valeur en visibilité
            if (value != null && value.ToString() == "END" && parameter.ToString() == "pause")
            {
                return Visibility.Collapsed;
            }
            else if (value != null && value.ToString() == "ACTIVE" && parameter.ToString() == "play")
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
