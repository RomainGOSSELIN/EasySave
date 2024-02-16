using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static EasySaveWPF.Model.Enum;

namespace EasySaveWPF.View
{
        public class JobTypeToEnumConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is JobTypeEnum jobType)
                {
                    return jobType;
                }

                return Binding.DoNothing;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is JobTypeEnum jobType)
                {
                    return jobType;
                }

                return Binding.DoNothing;
            }
    }
}
