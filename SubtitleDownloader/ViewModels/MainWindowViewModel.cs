using Prism.Mvvm;
using System.Windows;

namespace SubtitleDownloader.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private FlowDirection _MainFlowDirection;
        public FlowDirection MainFlowDirection
        {
            get => _MainFlowDirection;
            set => SetProperty(ref _MainFlowDirection, value);
        }
        public MainWindowViewModel()
        {
            MainFlowDirection = ((App)Application.Current).IsLanguageRTL() ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

    }
}
