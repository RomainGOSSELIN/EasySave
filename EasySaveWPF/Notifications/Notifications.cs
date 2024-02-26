using EasySaveWPF.Model;
using System;
using System.IO.Packaging;
using System.Windows;
using static EasySaveWPF.Model.Enum;

namespace EasySaveWPF.Notifications
{
    class Notifications
    {
        string? message;
        string? caption;
        MessageBoxButton button;
        MessageBoxImage icon;
        public void SourceDirNotExist(string SourceDir)
        {
            message = String.Format(Resources.Translation.source_directory_doesnt_exist, SourceDir);
            caption = Resources.Translation.error;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Error;
            MessageBox.Show(message, caption, button, icon);
        }

        public void TargetEmpty()
        {
            message = Resources.Translation.target_empty;
            caption = Resources.Translation.error;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Error;
            MessageBox.Show(message, caption, button, icon);
        }

        public void NameEmpty()
        {
            message = Resources.Translation.name_empty;
            caption = Resources.Translation.error;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Error;
            MessageBox.Show(message, caption, button, icon);
        }

        public void TargetSourceDifferent()
        {
            message = Resources.Translation.target_source_must_be_different;
            caption = Resources.Translation.error;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Error;
            MessageBox.Show(message, caption, button, icon);
        }

        public void CreateBackupjob(string Name, int? Id, string SourceDir, string TargetDir, JobTypeEnum Type)
        {
            message = String.Format(Resources.Translation.create_job_success, Name, Id, SourceDir, TargetDir, Type);
            caption = Resources.Translation.creation;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Information;
            MessageBox.Show(message, caption, button, icon);
        }

        public void BackupSuccess(List<BackupJob> executedJobs)
        {
            string listName = "";
            if (executedJobs.Count>1)
            {
                foreach (BackupJob job in executedJobs)
                {
                    listName = listName + job.Name + ", ";
                }
                message = String.Format(Resources.Translation.backups_success, listName);
                caption = Resources.Translation.success;
                button = MessageBoxButton.OK;
                icon = MessageBoxImage.Information;
                MessageBox.Show(message, caption, button, icon);

            }
            else
            {              
                message = String.Format(Resources.Translation.backup_success, executedJobs[0].Name);
                caption = Resources.Translation.success;
                button = MessageBoxButton.OK;
                icon = MessageBoxImage.Information;
                MessageBox.Show(message, caption, button, icon);
            }
        }

        public void BackupError(string Message)
        {
            message = String.Format(Resources.Translation.backup_error, Message);
            caption = Resources.Translation.error;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Error;
            MessageBox.Show(message, caption, button, icon);
        }

        public void BusinessSoftwareRunning()
        {
            message = Resources.Translation.business_software_running;
            caption = Resources.Translation.error;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Warning;
            MessageBox.Show(message, caption, button, icon, MessageBoxResult.Yes);
        }

        public void NoJob()
        {
            message = Resources.Translation.no_job_found;
            caption = Resources.Translation.error;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Error;
            MessageBox.Show(message, caption, button, icon);
        }

        public void InvalidExtension()
        {
            message = Resources.Translation.invalid_extension;
            caption = Resources.Translation.error;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Error;
            MessageBox.Show(message, caption, button, icon);
        }

        public void MultipleInstance()
        {
            message = Resources.Translation.mono_instance_app;
            caption = Resources.Translation.error;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Error;
            MessageBox.Show(message, caption, button, icon);
        }

        public void ChangesMade()
        {
            message = Resources.Translation.app_shutdown;
            caption = "";
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Information;
            MessageBox.Show(message, caption, button, icon);
        }

        public void NoChangesMade()
        {
            message = Resources.Translation.no_changes_made;
            caption = "";
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Information;
            MessageBox.Show(message, caption, button, icon);
        }

        public void CantDelete()
        {
            message = String.Format(Resources.Translation.cant_delete);
            caption = Resources.Translation.error;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Error;
            MessageBox.Show(message, caption, button, icon);
        }

        public void RangeNotValid()
        {
            message = Resources.Translation.range_not_valid;
            caption = Resources.Translation.error;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Error;
            MessageBox.Show(message, caption, button, icon);
        }
    }
}
