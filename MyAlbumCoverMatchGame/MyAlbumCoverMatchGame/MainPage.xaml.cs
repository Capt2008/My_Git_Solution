using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MyAlbumCoverMatchGame
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<Models.Song> DataSource;
        Models.SongManager DataManager { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            DataSource = new ObservableCollection<Models.Song>();
            DataManager = new Models.SongManager();
        }

        private async void GetSongButton_Click(object sender, RoutedEventArgs e)
        {
            DataSource.Clear();
            var songs =await DataManager.GetSongsAsync();
            songs.ForEach(i => DataSource.Add(i));
        }
    }
}
