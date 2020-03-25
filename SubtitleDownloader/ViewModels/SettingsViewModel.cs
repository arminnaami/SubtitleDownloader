using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Rasyidf.Localization;
using System;
using System.Windows.Controls;
using System.Windows.Forms;

namespace SubtitleDownloader.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        #region Property
        private string _CurrentStoreLocation;
        public string CurrentStoreLocation
        {
            get => _CurrentStoreLocation;
            set => SetProperty(ref _CurrentStoreLocation, value);
        }

        private string _currentServer;
        public string CurrentServer
        {
            get => _currentServer;
            set => SetProperty(ref _currentServer, value);
        }

        private string _currentLanguage;
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set => SetProperty(ref _currentLanguage, value);
        }

        private bool _getIsCheckedAutoDownload;
        public bool GetIsCheckedAutoDownload { get => _getIsCheckedAutoDownload; set => SetProperty(ref _getIsCheckedAutoDownload, value); }

        private bool _getIsCheckedShowNotification;
        public bool GetIsCheckedShowNotification { get => _getIsCheckedShowNotification; set => SetProperty(ref _getIsCheckedShowNotification, value); }


        private bool _getIsCheckedFileContextMenu;
        public bool GetIsCheckedFileContextMenu { get => _getIsCheckedFileContextMenu; set => SetProperty(ref _getIsCheckedFileContextMenu, value); }

        private bool _getIsCheckedFolderContextMenu;
        public bool GetIsCheckedFolderContextMenu { get => _getIsCheckedFolderContextMenu; set => SetProperty(ref _getIsCheckedFolderContextMenu, value); }

        private bool _getIsCheckedShowNotifyIcon;
        public bool GetIsCheckedShowNotifyIcon { get => _getIsCheckedShowNotifyIcon; set => SetProperty(ref _getIsCheckedShowNotifyIcon, value); }
        #endregion
        #region Command
        public DelegateCommand SelectFolderCommand { get; private set; }

        public DelegateCommand<object> AutoDownloadCommand { get; private set; }
        public DelegateCommand<object> ShowNotificationCommand { get; private set; }
        public DelegateCommand<object> AddToFileContextMenuCommand { get; private set; }
        public DelegateCommand<object> AddToFolderContextMenuCommand { get; private set; }
        public DelegateCommand<object> IsShowNotifyIconCommand { get; private set; }

        public DelegateCommand<SelectionChangedEventArgs> SubtitleLanguageCommand { get; private set; }
        public DelegateCommand<SelectionChangedEventArgs> ServerChangedCommand { get; private set; }
        #endregion

        public SettingsViewModel()
        {
            SelectFolderCommand = new DelegateCommand(SelectFolder);
            AutoDownloadCommand = new DelegateCommand<object>(AutoDownload);
            ShowNotificationCommand = new DelegateCommand<object>(ShowNotification);
            AddToFileContextMenuCommand = new DelegateCommand<object>(AddToFileContextMenu);
            AddToFolderContextMenuCommand = new DelegateCommand<object>(AddToFolderContextMenu);
            IsShowNotifyIconCommand = new DelegateCommand<object>(IsShowNotifyIcon);

            SubtitleLanguageCommand = new DelegateCommand<SelectionChangedEventArgs>(SubtitleLanguageChanged);
            ServerChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(ServerChanged);

            InitSettings();
        }

        private void InitSettings()
        {
            CurrentStoreLocation = GlobalData.Config.StoreLocation;

            GetIsCheckedAutoDownload = GlobalData.Config.IsAutoDownloadSubtitle;
            GetIsCheckedFileContextMenu = GlobalData.Config.IsContextMenuFile;
            GetIsCheckedFolderContextMenu = GlobalData.Config.IsContextMenuFolder;
            GetIsCheckedShowNotification = GlobalData.Config.IsShowNotification;
            GetIsCheckedShowNotifyIcon = GlobalData.Config.IsShowNotifyIcon;

            CurrentLanguage = string.Format(LocalizationService.GetString("1037", "Title", "زبان زیرنویس {0}"), GlobalData.Config.SubtitleLang);
            CurrentServer = string.Format(LocalizationService.GetString("1038", "Title", "سرور زیرنویس {0}"), GlobalData.Config.ServerUrl);
        }

        private void ServerChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }
            if (e.AddedItems[0] is ComboBoxItem item)
            {
                if (!item.Content.ToString().Equals(GlobalData.Config.ServerUrl))
                {
                    GlobalData.Config.ServerUrl = item.Content.ToString();
                    GlobalData.Save();
                    CurrentServer = string.Format(LocalizationService.GetString("1038", "Title", "سرور زیرنویس {0}"), GlobalData.Config.ServerUrl);
                }
            }
        }

        private void SubtitleLanguageChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }
            if (e.AddedItems[0] is ComboBoxItem item)
            {
                if (!item.Content.ToString().Equals(GlobalData.Config.SubtitleLang))
                {
                    GlobalData.Config.SubtitleLang = item.Content.ToString();
                    GlobalData.Save();
                    CurrentLanguage = string.Format(LocalizationService.GetString("1037", "Title", "زبان زیرنویس {0}"), GlobalData.Config.SubtitleLang);
                }
            }
        }

        #region ToggleButton
        private void ShowNotification(object isChecked)
        {
            if ((bool)isChecked != GlobalData.Config.IsShowNotification)
            {
                GlobalData.Config.IsShowNotification = (bool)isChecked;
                GlobalData.Save();
            }
        }

        private void IsShowNotifyIcon(object isChecked)
        {
            if ((bool)isChecked != GlobalData.Config.IsShowNotifyIcon)
            {
                GlobalData.Config.IsShowNotifyIcon = (bool)isChecked;
                GlobalData.Save();
            }
        }

        private void AddToFolderContextMenu(object isChecked)
        {
            if ((bool)isChecked != GlobalData.Config.IsContextMenuFolder)
            {
                GlobalData.Config.IsContextMenuFolder = (bool)isChecked;
                GlobalData.Save();
                RegisterContextMenu(true, !(bool)isChecked);
            }
        }

        private void AddToFileContextMenu(object isChecked)
        {
            if ((bool)isChecked != GlobalData.Config.IsContextMenuFile)
            {
                GlobalData.Config.IsContextMenuFile = (bool)isChecked;
                GlobalData.Save();
                RegisterContextMenu(false, !(bool)isChecked);
            }
        }

        private void AutoDownload(object isChecked)
        {
            if ((bool)isChecked != GlobalData.Config.IsAutoDownloadSubtitle)
            {
                GlobalData.Config.IsAutoDownloadSubtitle = (bool)isChecked;
                GlobalData.Save();
            }
        }
        #endregion

        private void SelectFolder()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    CurrentStoreLocation = dialog.SelectedPath + @"\";
                    GlobalData.Config.StoreLocation = CurrentStoreLocation;
                    GlobalData.Save();
                }
            }
        }

        private void RegisterContextMenu(bool IsFolder, bool IsDelete = false)
        {
            try
            {
                if (IsDelete)
                {
                    if (IsFolder)
                    {
                        RegistryKey regFolderKeyOpen = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\directory\shell\", true);
                        regFolderKeyOpen.DeleteSubKeyTree("Get Subtitle");
                        regFolderKeyOpen.DeleteSubKeyTree("Get Subtitle with World Subtitle");
                    }
                    else
                    {
                        RegistryKey regFileKeyOpen = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\*\shell\", true);
                        regFileKeyOpen.DeleteSubKeyTree("Get Subtitle");
                        regFileKeyOpen.DeleteSubKeyTree("Get Subtitle with World Subtitle");
                    }
                }
                else
                {
                    if (IsFolder)
                    {
                        RegistryKey regFolderOpen = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\directory\shell\Get Subtitle\command\", true);
                        if (regFolderOpen == null)
                        {
                            //Subscene
                            RegistryKey regFolderKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\directory\shell\Get Subtitle\command\");
                            regFolderKey.SetValue("", $"\"{System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName}\" \"%1\"");
                        }
                    }
                    else
                    {
                        RegistryKey regFileOpen = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\*\shell\Get Subtitle\command\", true);
                        if (regFileOpen == null)
                        {
                            //Subscene
                            RegistryKey regFileKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\*\shell\Get Subtitle\command\");
                            regFileKey.SetValue("", $"\"{System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName}\" \"%1\"");
                        }
                    }
                }
            }
            catch (ArgumentException) { }
            catch (NullReferenceException) { }
        }
    }
}
