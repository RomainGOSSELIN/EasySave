using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;

// Pour l'explorateur de fichiers
using Microsoft.Win32;

namespace EasySaveWPF.View
{
    public partial class Settings : UserControl
    {
        private bool changesMade = false;
        Notifications.Notifications notifications = new Notifications.Notifications();
        private ServiceProvider serviceProvider;


        public Settings()
        {
            InitializeComponent();

            this.DataContext = new ViewModel.SettingsViewModel();
            SetRadioButtonLanguageState();
            SetRadioButtonThemeState();
            numberTextBox.Text = "0";

        }

        private void RadioButtonLightTheme_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void RadioButtonDarkTheme_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void LogsFormatXML_Checked(object sender, RoutedEventArgs e)
        {
            changesMade = true;
        }

        private void LogsFormatJSON_Checked(object sender, RoutedEventArgs e)
        {
            changesMade = true;
        }

        private void Language_Choice(object sender, RoutedEventArgs e)
        {
            string selectedLang = ((RadioButton)sender).Content.ToString();
            //Check if selected language is different from current language
            if (selectedLang != EasySaveWPF.Resources.TranslationSettings.Default.LanguageCode)
            {
                //Set the default language with the selected
                EasySaveWPF.Resources.TranslationSettings.Default.LanguageCode = selectedLang;
                changesMade = true;
            }
        }

        private void SetRadioButtonLanguageState()
        {
            //Get the language parameter
            string laguageSelected = EasySaveWPF.Resources.TranslationSettings.Default.LanguageCode;

            //Check the good radioButton
            Language_Choice_French.IsChecked = laguageSelected == Language_Choice_French.Content.ToString();
            Language_Choice_English.IsChecked = laguageSelected == Language_Choice_English.Content.ToString();
            Language_Choice_Deutsch.IsChecked = laguageSelected == Language_Choice_Deutsch.Content.ToString();
            Language_Choice_Italian.IsChecked = laguageSelected == Language_Choice_Italian.Content.ToString();
            Language_Choice_Spanish.IsChecked = laguageSelected == Language_Choice_Spanish.Content.ToString();
        }

        private void PathBusinessSoftware_Changed(object sender, TextChangedEventArgs e)
        {
            changesMade = true;
        }

        private void Files_Explorer(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private const string ExtensionSeparator = " ";

        private void AddExtension_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputExtension.Text))
            {
                var extension = InputExtension.Text.Trim();
                if (extension.StartsWith(".") && !extension.Equals(".") && !extension.Contains(" "))
                {
                    var extensions = GetExtensionsList();
                    if (!extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                    {
                        extensions.Add(extension);
                        UpdateListExtensions(extensions);
                        changesMade = true;
                    }
                }
                else
                {
                    notifications.InvalidExtension();
                }
            }
            InputExtension.Text = "";
        }

        private void RemoveExtension_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputExtension.Text))
            {
                var extension = InputExtension.Text.Trim();
                if (extension.StartsWith(".") && !extension.Equals(".") && !extension.Contains(" "))
                {
                    var extensions = GetExtensionsList();
                    extensions.Remove(extension);
                    UpdateListExtensions(extensions);
                    changesMade = true;
                }
                else
                {
                    notifications.InvalidExtension();
                }
            }
            InputExtension.Text = "";

        }


        private void ClearListExtension_Click(object sender, RoutedEventArgs e)
        {
            ListExtension.Text = "";
            InputExtension.Text = "";
            changesMade = true;
        }

        private List<string> GetExtensionsList()
        {
            return ListExtension.Text.Split(new[] { ExtensionSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void UpdateListExtensions(List<string> extensions)
        {
            ListExtension.Text = string.Join(ExtensionSeparator, extensions);
        }


        // Méthode pour ouvrir l'explorateur de fichiers et permettre à l'utilisateur de sélectionner un fichier
        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Sélectionnez un fichier";
            openFileDialog.Filter = "exe files (*.exe)|*.exe"; 

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = System.IO.Path.GetFileName(openFileDialog.FileName);
                PathBusinessSoftware.Text = selectedFile;
            }
        }

        private void Theme_Choice(object sender, RoutedEventArgs e)
        {
            string selectedTheme = ((RadioButton)sender).Content.ToString();

            EasySaveWPF.Theme.Theme.Default.selectedTheme = selectedTheme;
            EasySaveWPF.Theme.AppTheme.ChangeTheme(new Uri("Theme/" + selectedTheme + ".xaml", UriKind.Relative));
        }

        private void SetRadioButtonThemeState()
        {
            //Get the theme parameter
            string themeSelected = EasySaveWPF.Theme.Theme.Default.selectedTheme;

            //Check the good radioButton
            radioButtonDarkTheme.IsChecked = themeSelected == radioButtonDarkTheme.Content.ToString();
            radioButtonLightTheme.IsChecked = themeSelected == radioButtonLightTheme.Content.ToString();
        }

        private void AddPriorityExtension_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputPriorityExtension.Text))
            {
                var extension = InputPriorityExtension.Text.Trim();
                if (extension.StartsWith(".") && !extension.Equals(".") && !extension.Contains(" "))
                {
                    var extensions = GetPriorityExtensionsList();
                    if (!extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                    {
                        extensions.Add(extension);
                        UpdatePriorityListExtensions(extensions);
                        changesMade = true;
                    }
                }
                else
                {
                    notifications.InvalidExtension();
                }
            }
            InputPriorityExtension.Text = "";
        }

        private void RemovePriorityExtension_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputPriorityExtension.Text))
            {
                var extension = InputPriorityExtension.Text.Trim();
                if (extension.StartsWith(".") && !extension.Equals(".") && !extension.Contains(" "))
                {
                    var extensions = GetPriorityExtensionsList();
                    extensions.Remove(extension);
                    UpdatePriorityListExtensions(extensions);
                    changesMade = true;
                }
                else
                {
                    notifications.InvalidExtension();
                }
            }
            InputPriorityExtension.Text = "";
        }

        private void ClearPriorityListExtension_Click(object sender, RoutedEventArgs e)
        {
            ListPriorityExtension.Text = "";
            InputPriorityExtension.Text = "";
            changesMade = true;
        }

        private List<string> GetPriorityExtensionsList()
        {
            return ListPriorityExtension.Text.Split(new[] { ExtensionSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void UpdatePriorityListExtensions(List<string> extensions)
        {
            ListPriorityExtension.Text = string.Join(ExtensionSeparator, extensions);
        }

        private void ApplyChangesAndRestart_Click(object sender, RoutedEventArgs e)
        {
            if (changesMade)
            {
                Properties.Settings.Default.Save();
                EasySaveWPF.Theme.Theme.Default.Save();
                EasySaveWPF.Resources.TranslationSettings.Default.Save();

                notifications.ChangesMade();
                
            }
            else
            {
                notifications.NoChangesMade();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Utiliser une expression régulière pour n'accepter que les chiffres
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void IncreaseButtonClick(object sender, RoutedEventArgs e)
        {
            // Incrémenter le nombre dans la TextBox
            if (int.TryParse(numberTextBox.Text, out int number))
            {
                number++;
                numberTextBox.Text = number.ToString();
            }
        }

        private void DecreaseButtonClick(object sender, RoutedEventArgs e)
        {
            // Décrémenter le nombre dans la TextBox
            if (int.TryParse(numberTextBox.Text, out int number))
            {
                // Vérifier si le nombre est supérieur à 0 avant de le décrémenter
                if (number > 0)
                {
                    number--;
                    numberTextBox.Text = number.ToString();
                }
            }
        }
    }

}