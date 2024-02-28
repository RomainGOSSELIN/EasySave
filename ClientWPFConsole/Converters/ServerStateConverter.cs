using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ClientWPFConsole.Converters
{
    class ServerStateConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if(parameter is string sender)
                {
                    if (sender == "value")
                        return boolValue ? "Connecté" : "Déconnecté";
                    if (sender == "button")
                        return boolValue ? "Déconnecter" : "Connecter";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
