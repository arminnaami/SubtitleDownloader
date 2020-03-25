using HandyControl.Data;
using HandyControl.Tools;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Prism.Ioc;
using Prism.Regions;
using Rasyidf.Localization;
using SubtitleDownloader.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;

namespace SubtitleDownloader
{
    public partial class App
    {
        public static string[] WindowsContextMenuArgument = { string.Empty, string.Empty };

        private readonly List<string> wordsToRemove = "DD5 YTS TURKISH VIDEOFLIX Gisaengchung KOREAN 8CH BluRay Hdcam HDCAM . - XviD AC3 EVO WEBRip FGT MP3 CMRG Pahe 10bit 720p 1080p 480p WEB-DL H264 H265 x264 x265 800MB 900MB HEVC PSA RARBG 6CH 2CH CAMRip Rip AVS RMX HDTV RMTeam mSD SVA MkvCage MeGusta TBS AMZN DDP5.1 DDP5 SHITBOX NITRO WEB DL 1080 720 480 MrMovie BWBP NTG "
           .Split(' ').ToList();

        public string RemoveJunkString(string stringToClean)
        {
            string cleaned = Regex.Replace(stringToClean, "\\b" + string.Join("\\b|\\b", wordsToRemove) + "\\b", " ");
            cleaned = Regex.Replace(cleaned, @"S[0-9].{1}E[0-9].{1}", ""); // remove SXXEXX ==> X is 0-9
            cleaned = Regex.Replace(cleaned, @"(\[[^\]]*\])|(\([^\)]*\))", ""); // remove between () and []
            cleaned = Regex.Replace(cleaned, "[ ]{2,}", " "); // remove space [More than 2 space] and replace with one space

            return cleaned.Trim();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length > 0)
            {
                string NameFromContextMenu = RemoveJunkString(Path.GetFileNameWithoutExtension(e.Args[0]));

                WindowsContextMenuArgument[0] = NameFromContextMenu;
                WindowsContextMenuArgument[1] = e.Args[0].Replace(Path.GetFileName(e.Args[0]), "");

                Container.Resolve<IRegionManager>().RequestNavigate("ContentRegion", "Subscene");
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        protected override Window CreateShell()
        {
            GlobalData.Init();
            if (GlobalData.Config.Skin != SkinType.Default)
            {
                UpdateSkin(GlobalData.Config.Skin);
            }
            UpdateLanguage(GlobalData.Config.UILang, true);
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainContent>();
            containerRegistry.RegisterForNavigation<LeftMainContent>();
            containerRegistry.RegisterForNavigation<About>();
            containerRegistry.RegisterForNavigation<Settings>();
            containerRegistry.RegisterForNavigation<Updater>();
            containerRegistry.RegisterForNavigation<PopularSeries>();
            containerRegistry.RegisterForNavigation<Subscene>();
            containerRegistry.RegisterForNavigation<SubsceneDownload>();
        }

        internal void UpdateSkin(SkinType skin)
        {
            Resources.MergedDictionaries.Add(ResourceHelper.GetSkin(skin));
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
            Current.MainWindow?.OnApplyTemplate();
        }

        public override void Initialize()
        {
            base.Initialize();

            //init Appcenter Crash Reporter
            AppCenter.Start("3770b372-60d5-49a1-8340-36a13ae5fb71",
                   typeof(Analytics), typeof(Crashes));
            AppCenter.Start("3770b372-60d5-49a1-8340-36a13ae5fb71",
                               typeof(Analytics), typeof(Crashes));
        }

        internal void UpdateLanguage(string cultureName, bool isInit)
        {
            if (isInit)
            {
                LocalizationService.Current.Initialize("Assets", cultureName);
            }
            else
            {
                CultureInfo culture = new CultureInfo(cultureName);
                LocalizationService.Current.ChangeLanguage(LocalizationDictionary.GetResources(culture));
            }
            ConfigHelper.Instance.SetLang(cultureName);
        }

        internal bool IsLanguageRTL()
        {
            string culture = LocalizationService.Current.LanguagePack.Culture.Name;
            return culture.Equals("fa-IR");
        }
    }
}
