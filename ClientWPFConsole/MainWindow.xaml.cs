using ClientWPFConsole.Model;
using ClientWPFConsole.ViewModel;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using static EasySaveWPF.Model.Enum;

namespace ClientWPFConsole
{
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();

        }






    }
}
