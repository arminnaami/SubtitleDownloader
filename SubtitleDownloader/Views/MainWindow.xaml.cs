using HandyControl.Controls;
using HandyControl.Data;
using Rasyidf.Localization;
using SubtitleDownloader.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace SubtitleDownloader.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Change Skin and Language
        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e)
        {
            PopupConfig.IsOpen = true;
        }

        private void ButtonSkins_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button && button.Tag is SkinType tag)
            {
                PopupConfig.IsOpen = false;
                if (tag.Equals(GlobalData.Config.Skin))
                {
                    return;
                }

                GlobalData.Config.Skin = tag;
                GlobalData.Save();
                ((App)Application.Current).UpdateSkin(tag);
            }
        }

        private void ButtonLangs_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button && button.Tag is string tag)
            {
                PopupConfig.IsOpen = false;
                if (tag.Equals(GlobalData.Config.UILang))
                {
                    return;
                }

                Growl.Ask(LocalizationService.GetString("1003", "Text", "آیا مایل به تغییر زبان برنامه هستید؟"), b =>
                {
                    if (!b)
                    {
                        return true;
                    }

                    GlobalData.Config.UILang = tag;
                    GlobalData.Save();
                    ((App)Application.Current).UpdateLanguage(tag, false);
                    ((MainWindowViewModel)(DataContext)).MainFlowDirection = ((App)Application.Current).IsLanguageRTL() ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                    return true;
                });
            }
        }

        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (GlobalData.Config.IsShowNotifyIcon)
            {
                if (GlobalData.Config.IsFirstRun)
                {
                    MessageBoxResult result = HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                    {
                        Message = LocalizationService.GetString("1004", "Text", "برنامه در پس زمینه در حال اجرا خواهد بود و از سیستم ترای قابل دسترس خواهد بود و این پنجره به جای بستن، مخفی خواهد شد. آیا موافقید؟"),
                        Caption = LocalizationService.GetString("1000", "Title", "Subtitle Downloader"),
                        Button = MessageBoxButton.YesNo,
                        IconBrushKey = ResourceToken.AccentBrush,
                        IconKey = ResourceToken.InfoGeometry
                    });
                    if (result == MessageBoxResult.Yes)
                    {
                        Hide();
                        e.Cancel = true;
                        GlobalData.Config.IsFirstRun = false;
                        GlobalData.Save();
                    }
                    else
                    {
                        base.OnClosing(e);
                    }
                }
                else
                {
                    Hide();
                    e.Cancel = true;
                }
            }
            else
            {
                base.OnClosing(e);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
