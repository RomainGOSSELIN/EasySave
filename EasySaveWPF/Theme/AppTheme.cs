using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace EasySaveWPF.Theme
{
    public class AppTheme
    {
        public static void ChangeTheme(Uri themeUri)
        {
            ResourceDictionary Theme = new ResourceDictionary() {  Source = themeUri };
            Uri LightUri = new Uri("Theme/Light.xaml", UriKind.Relative);
            Uri DarkUri = new Uri("Theme/Dark.xaml", UriKind.Relative);
            ResourceDictionary oldTheme = new ResourceDictionary() { Source = (themeUri == LightUri ? DarkUri : LightUri) };
            App.Current.Resources.MergedDictionaries.Remove(oldTheme);
            App.Current.Resources.MergedDictionaries.Add(Theme);
        }
    }
}
