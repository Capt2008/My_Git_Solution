using MyAlbumCoverMatchGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyAlbumCoverMatchGame
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PrePage : Page
    {
        public PrePage()
        {
            this.InitializeComponent();
        }

        private async void StartNewButton_Click(object sender, RoutedEventArgs e)
        {
            await SongManager.RemoveNoAvailabeItem(true);
            var task = SongManager.InitialXmlAsync();
            Frame.Navigate(typeof(MainPage), task);

        }

        private async void GoonButton_Click(object sender, RoutedEventArgs e)
        {
            await SongManager.RemoveNoAvailabeItem(false);
            var task = SongManager.InitialXmlAsync();
            Frame.Navigate(typeof(MainPage), task);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var b = SongManager.IsAppFirstRun();
            if (b)  //  xml is available
            {
                GuidPanel.Visibility = Visibility.Visible;
                InitialPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                //  初始化游戏xml,完成后开始游戏
                GuidPanel.Visibility = Visibility.Collapsed;
                InitialPanel.Visibility = Visibility.Visible;
                LoadingRing.IsActive = true;
                await SongManager.InitialXmlAsync();
                LoadingRing.IsActive = false;
                Frame.Navigate(typeof(MainPage));
            }
        }
    }
}
