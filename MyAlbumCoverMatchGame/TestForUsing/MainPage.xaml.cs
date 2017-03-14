using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

/// <summary>
/// 使用MediaElement播放音乐文件时，发现每次程序启动，点击播放按钮的第一次都不好用。于是开了这个Demo,专门测试这块。
/// 结果是是using()把stream给释放掉的太快了，前台Element会Get不到（或不完整）。最后在using()中加上手动延迟完美解决。
/// </summary>
namespace TestForUsing
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<StorageFile> SongList { get; set; } = new List<StorageFile>();
        int i = 0;
        public MainPage()
        {
            this.InitializeComponent();
            MusicPlayer.CurrentStateChanged += async (sender, e) => await ShowMessage(MusicPlayer.CurrentState.ToString());
        }



        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var stream = await SongList[i].OpenReadAsync())
            {
                MusicPlayer.SetSource(stream, stream.ContentType);
                await Task.Delay(10);   // Got it, right here!! Manual delay disposing!
            }
            i = ++i % 5;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var stream = await SongList[i].OpenReadAsync();
            MusicPlayer.SetSource(stream, stream.ContentType);
            MusicPlayer.Play();
            i = ++i % 5;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<StorageFile> songList = await KnownFolders.MusicLibrary.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByName, 0, 5);
            foreach (var item in songList)
            {
                SongList.Add(item);
            }
            await ShowMessage("音乐加载成功！");
        }

        private async Task ShowMessage(string message)
        {
            MessageText.Text = message;
            await Task.Delay(2000);
            MessageText.Text = string.Empty;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MusicPlayer.Stop();
            i = 0;
        }
    }
}
