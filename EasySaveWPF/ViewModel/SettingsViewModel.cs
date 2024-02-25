using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveWPF.ViewModel
{
    public class SettingsViewModel : ViewModelBase
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
			}
		} 

		private string _priorityFiles;
		public string PriorityFiles
        {
            get
			{
                return _priorityFiles;
            }
            set
			{
                _priorityFiles = value;
                OnPropertyChanged(nameof(PriorityFiles));
                Properties.Settings.Default.PriorityFiles = value;
            }
        }
		#endregion

		public SettingsViewModel()
		{
			BusinessSoftware = Properties.Settings.Default.BusinessSoftwarePath;
			LogType = Properties.Settings.Default.LogType;
            FilesToEncrypt = Properties.Settings.Default.FilesToEncrypt;
            PriorityFiles = Properties.Settings.Default.PriorityFiles;
        }

	}
}
