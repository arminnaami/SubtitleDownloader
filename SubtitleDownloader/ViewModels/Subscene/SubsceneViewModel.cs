using HandyControl.Controls;
using HandyControl.Data;
using HtmlAgilityPack;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Rasyidf.Localization;
using SubtitleDownloader.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MessageBox = HandyControl.Controls.MessageBox;

namespace SubtitleDownloader.ViewModels
{
    public class SubsceneViewModel : BindableBase, INavigationAware
    {
        private const string SearchAPI = "{0}/subtitles/searchbytitle?query={1}&l=";

        private readonly IRegionManager _regionManager;

        #region Property
        private Visibility _visibility = Visibility.Hidden;
        public Visibility ContentVisibility
        {
            get => _visibility;
            set => SetProperty(ref _visibility, value);
        }

        private ObservableCollection<SubsceneModel> _dataList = new ObservableCollection<SubsceneModel>();
        public ObservableCollection<SubsceneModel> DataList
        {
            get => _dataList;
            set => SetProperty(ref _dataList, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

        #endregion

        #region Command
        public DelegateCommand<SelectionChangedEventArgs> OpenSubtitlePageCommand { get; private set; }

        public DelegateCommand<FunctionEventArgs<string>> OnSearchStartedCommand { get; private set; }

        #endregion

        public SubsceneViewModel()
        {

        }

        public SubsceneViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            OnSearchStartedCommand = new DelegateCommand<FunctionEventArgs<string>>(OnSearchStarted);
            OpenSubtitlePageCommand = new DelegateCommand<SelectionChangedEventArgs>(OpenSubtitlePage);

        }

        private void OpenSubtitlePage(SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] is SubsceneModel item)
            {
                if (item.Link != null)
                {
                    NavigationParameters parameters = new NavigationParameters
                    { { "link_key", item.Link } };

                    _regionManager.RequestNavigate("ContentRegion", "SubsceneDownload", parameters);
                }
            }
        }

        private async void OnSearchStarted(FunctionEventArgs<string> e)
        {
            try
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    return;
                }

                ContentVisibility = Visibility.Visible;
                IsBusy = true;
                string url = string.Format(SearchAPI, GlobalData.Config.ServerUrl, SearchText);
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = await web.LoadFromWebAsync(url);

                HtmlNodeCollection repeater = doc.DocumentNode.SelectNodes("//div[@class='title']");
                if (repeater == null)
                {
                    MessageBox.Error(LocalizationService.GetString("1040", "Text", "زیرنویس موردنظر پیدا نشد"));
                }
                else
                {
                    DataList?.Clear();
                    foreach (HtmlNode node in repeater)
                    {
                        if (node.InnerText.Contains("OFFER POST"))
                        {
                            continue;
                        }
                        SubsceneModel item = new SubsceneModel { Link = node.SelectSingleNode(".//a")?.Attributes["href"]?.Value + $"/{GlobalData.Config.SubtitleLang}/", Name = node.InnerText.Trim() };
                        DataList.Add(item);
                    }
                }
                IsBusy = false;
            }
            catch (ArgumentOutOfRangeException) { }
            catch (ArgumentNullException) { }
            catch (System.NullReferenceException) { }
            catch (System.Net.WebException ex)
            {
                Growl.Error(LocalizationService.GetString("1041", "Text", "سرور در دسترس نیست") + "\n" + ex.Message);
            }
            catch (System.Net.Http.HttpRequestException hx)
            {
                Growl.Error(LocalizationService.GetString("1041", "Text", "سرور در دسترس نیست") + "\n" + hx.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            string name = navigationContext.Parameters["name_key"] as string;
            if (!string.IsNullOrEmpty(name))
            {
                SearchText = name;
                OnSearchStarted(null);
            }

            if (!string.IsNullOrEmpty(App.WindowsContextMenuArgument[0]))
            {
                SearchText = App.WindowsContextMenuArgument[0];
                OnSearchStarted(null);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
