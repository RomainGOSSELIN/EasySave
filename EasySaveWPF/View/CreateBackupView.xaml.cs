using Microsoft.Win32;
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

namespace EasySaveWPF.View
{
    public partial class CreateBackupView : UserControl
    {
        public CreateBackupView()
        {
            InitializeComponent();
            //this.DataContext = new ViewModel.CreateBackupViewModel();
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void Files_Explorer(object sender, RoutedEventArgs e)
        {
            OpenFolder(BackupSource_Textbox);
        }
        private void Files_Explorer_2(object sender, RoutedEventArgs e)
        {
            OpenFolder(BackupDestination_Textbox);
        }

        private void OpenFolder( TextBox boxPath )
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Title = "Sélectionnez un fichier";
          

            if (openFolderDialog.ShowDialog() == true)
            {
                string filePath = openFolderDialog.FolderName;
                boxPath.Text = filePath; 
            }
        }


    }
}
