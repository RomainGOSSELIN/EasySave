using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
// Pour l'explorateur de fichiers
using Microsoft.Win32;

namespace EasySaveWPF.View
{
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();

            this.DataContext = new ViewModel.SettingsViewModel();
        }

        private void RadioButtonLightTheme_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void RadioButtonDarkTheme_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void LogsFormatXML_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void LogsFormatJSON_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Language_Choice(object sender, RoutedEventArgs e)
        {

        }

        private void PathBusinessSoftware_Changed(object sender, TextChangedEventArgs e)
        {

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
                    }
                }
                else
                {
                    // Afficher un message d'erreur indiquant que l'extension est invalide
                    MessageBox.Show("Invalid extension. Extension should start with a dot (.) and not contain spaces.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemoveExtension_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputExtension.Text))
            {
                var extension = InputExtension.Text.Trim();
                if (extension.StartsWith("."))
                {
                    var extensions = GetExtensionsList();
                    extensions.Remove(extension);
                    UpdateListExtensions(extensions);
                }
                else
                {
                    // Afficher un message d'erreur indiquant que l'extension est invalide
                    MessageBox.Show("Invalid extension. Extension should start with a dot (.)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void ClearListExtension_Click(object sender, RoutedEventArgs e)
        {
            ListExtension.Text = "";
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
            openFileDialog.Filter = "Tous les fichiers (*.*)|*.*"; // Exemple de filtre pour tous les fichiers

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                PathBusinessSoftware.Text = filePath; // Affecte le chemin du fichier à la TextBox
            }
        }
    }   
}