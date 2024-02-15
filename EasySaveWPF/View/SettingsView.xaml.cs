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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Language_Choice(object sender, RoutedEventArgs e)
        {

        }

        private void Files_Explorer(object sender, RoutedEventArgs e)
        {
            OpenFile(); // Appelle la méthode OpenFile lorsque le bouton est cliqué
        }

        private void AddText_Click(object sender, RoutedEventArgs e)
        {
            // Vérifie si le TextBlock contient déjà le texte de la TextBox
            // Utilisez StringComparison.OrdinalIgnoreCase pour une comparaison insensible à la casse
            if (!outputTextBlock.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Contains(inputTextBox.Text, StringComparer.OrdinalIgnoreCase))
            {
                // Ajoute le contenu de la TextBox au TextBlock, avec un espace si le TextBlock n'est pas vide
                outputTextBlock.Text += (outputTextBlock.Text.Length > 0 ? " " : "") + inputTextBox.Text;
            }
        }

        private void RemoveText_Click(object sender, RoutedEventArgs e)
        {
            // Obtient un tableau des mots dans le TextBlock
            var words = outputTextBlock.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Cherche le texte de la TextBox dans la liste des mots
            var index = words.FindIndex(word => word.Equals(inputTextBox.Text, StringComparison.OrdinalIgnoreCase));

            if (index != -1)
            {
                // Supprime le mot de la liste
                words.RemoveAt(index);

                // Reconstruit le texte du TextBlock à partir de la liste mise à jour des mots
                outputTextBlock.Text = string.Join(" ", words);
            }
        }

        private void ClearText_Click(object sender, RoutedEventArgs e)
        {
            // Efface le contenu du TextBlock
            outputTextBlock.Text = "";
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
                PathTextBox.Text = filePath; // Affecte le chemin du fichier à la TextBox
            }
        }
    }   
}