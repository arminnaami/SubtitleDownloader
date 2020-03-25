using Prism.Mvvm;
using Rasyidf.Localization;
using System.Reflection;

namespace SubtitleDownloader.ViewModels
{
    public class AboutViewModel : BindableBase
    {
        private string _version;
        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }
        public AboutViewModel()
        {
            Version = string.Format(LocalizationService.GetString("1039", "Text", "نسخه {0}"), Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
    }
}
