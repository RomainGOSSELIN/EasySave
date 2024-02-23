using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveWPF.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {
		#region Propchanges
		private string _businessSoftware;
		public string BusinessSoftware
		{
			get
			{
				return _businessSoftware;
			}
			set
			{
				_businessSoftware = value;
				OnPropertyChanged(nameof(BusinessSoftware));
				Properties.Settings.Default.BusinessSoftwarePath = value;
				Properties.Settings.Default.Save();
			}
		}

		private string _logType;
		public string LogType
		{
			get
			{
				return _logType;
			}
			set
			{
				_logType = value;
				OnPropertyChanged(nameof(LogType));
				Properties.Settings.Default.LogType = value;
				Properties.Settings.Default.LogFilePath = $".\\Log.{LogType}";
				Properties.Settings.Default.Save();
			}
		}

		private string _filesToEncrypt;
		public string FilesToEncrypt
		{
			get
			{
				return _filesToEncrypt;
			}
			set
			{
				_filesToEncrypt = value;
				OnPropertyChanged(nameof(FilesToEncrypt));
				Properties.Settings.Default.FilesToEncrypt = value;
				Properties.Settings.Default.Save();
			}
		} 
		#endregion

		public SettingsViewModel()
		{
			BusinessSoftware = Properties.Settings.Default.BusinessSoftwarePath;
			LogType = Properties.Settings.Default.LogType;
            FilesToEncrypt = Properties.Settings.Default.FilesToEncrypt;

        }

	}
}
