using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static EasySaveWPF.Model.Enum;

namespace EasySaveWPF.View
{
    public class JobTypeToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is JobTypeEnum))
                return DependencyProperty.UnsetValue;

            JobTypeEnum jobType = (JobTypeEnum)value;

            if (parameter == null || !Enum.IsDefined(typeof(JobTypeEnum), parameter))
                return DependencyProperty.UnsetValue;

            JobTypeEnum targetTypeValue = (JobTypeEnum)Enum.Parse(typeof(JobTypeEnum), parameter.ToString());

            return jobType == targetTypeValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool) || parameter == null || !Enum.IsDefined(typeof(JobTypeEnum), parameter))
                return DependencyProperty.UnsetValue;

            bool isChecked = (bool)value;
            JobTypeEnum targetTypeValue = (JobTypeEnum)Enum.Parse(typeof(JobTypeEnum), parameter.ToString());

            return isChecked ? targetTypeValue : DependencyProperty.UnsetValue;
        }
    }
}
