using MyAlbumCoverMatchGame.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static MyAlbumCoverMatchGame.Models.SongManager;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MyAlbumCoverMatchGame
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<Album> AlbumsList;
        private ObservableCollection<Models.Song> SongList { get; set; }=new ObservableCollection<Song>();
        private List<Song> _correctSongs { get; set; } = new List<Song>();
        private int _roundNum;  //  For markup game round
        private Song CurrentSong; //   For mark the playing song in list       
        private bool _gameStatus = false; //  what is next operation of the game: true-play Music or false-cooldown
        private int _ttlScore;  

        public MainPage()
        {
            this.InitializeComponent();           
            AlbumsList = GetAlbumDataSource();
            _roundNum = 0;
            SongPlayer.CurrentStateChanged += AfterSongStoped;            
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var task = e.Parameter as Task;
            if (task!=null)
            {
                await task;
            } 
            UpDateMessageGrid.Visibility = Visibility.Visible;
            UpDateMessage.Text = "Music Data updated successfully！";
            await Task.Delay(4000);
            UpDateMessageGrid.Visibility = Visibility.Collapsed;
            UpDateMessage.Text = string.Empty;
        }

        private async void GetSongButton_Click(object sender, RoutedEventArgs e)
        {
            SongList.Clear();
            ClearSongInfo();
            GameDescTextBlock.Text = string.Empty;
            _roundNum = 0;
            _ttlScore = 0;
            GetSongButton.Visibility = Visibility.Collapsed;
            var songs = await SongManager.GetSongsAsync(AlbumsList);
            songs.ForEach(i => SongList.Add(i));
            CoolDownStart();
        }

        private void CoverImageGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (SongPlayer.CurrentState!=MediaElementState.Playing) return;
            if (((Song)e.ClickedItem).Thumbnail.UriSource.AbsolutePath.Contains("Assets")) return;

            //  Judge the user selection, 
            //  Show UI effects about result of the user selection
            //  Change the user scores
            if (CurrentSong.Equals((Song)e.ClickedItem))
            {
                CurrentSong.GameMark = true;
                _correctSongs.Add(CurrentSong);
                ((Song)e.ClickedItem).Thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/correct.png"));
                _ttlScore +=(int) CountDownBar.Value * 2;
            }
            else
            {
                ((Song)e.ClickedItem).Thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/incorrect.png"));
                _ttlScore += (int)CountDownBar.Value * -2;
            }           
            SongPlayer.Stop();
        }

        private void ClearSongInfo()
        {
            SongNameText.Text =
            AlbumTest.Text =
            ArtistText.Text =
            ScoreText.Text = string.Empty;
        }

        private void CoolDownStart()
        {
            CountDownBar.Value = 100;
            CountDownBar.Foreground = new SolidColorBrush(Colors.Turquoise);
            GameDescTextBlock.Text = $"Round {++_roundNum} is coming...";
            CountDownAnimation.Begin();
            _gameStatus = true;
        }

        private async Task GameStarted()
        {
            ClearSongInfo();
            CountDownBar.Value = 100;
            CountDownBar.Foreground = new SolidColorBrush(Colors.HotPink);
            GameDescTextBlock.Text = $"Round {_roundNum} ";
            await PlaySong();
            CountDownAnimation.Begin();
            _gameStatus = false;
        }

        private async Task PlaySong()
        {
            var list = SongList.Where(s => s.GameMark == null);
            Random rdm = new Random();
            int r = rdm.Next(list.Count());
            CurrentSong = list.ElementAt(r);            
            var file = await StorageFile.GetFileFromPathAsync(CurrentSong.UriStr);
            SongPlayer.SetSource(await file.OpenReadAsync(), file.ContentType);
            CurrentSong.GameMark = false;
        }

        //private void StartButton_Click(object sender, RoutedEventArgs e)
        //{
        //    CoolDownStart();
        //}

        private async void AfterSongStoped(object sender, RoutedEventArgs e)
        {
            if (SongPlayer.CurrentState != MediaElementState.Stopped|| _gameStatus == true) return;
            
            //  Show Song info
            SongNameText.Text = CurrentSong.Name;
            AlbumTest.Text =CurrentSong.Album.Name;
            ArtistText.Text = CurrentSong.Artist;
            ScoreText.Text = _ttlScore.ToString();
            
            if (_roundNum > 4) //   wether go on game or not?
            {
                CountDownAnimation.Stop();
                CountDownBar.Value = 100;
                GetSongButton.Visibility = Visibility.Visible;
                string result = "correct songs: ";
                var correctSongs = SongList.Where(song => song.GameMark == true);
                foreach (var song in correctSongs)
                {
                    result += $" [{song.Name}] ";                  
                    song.AfterPlayed();
                }
                GameDescTextBlock.Text = "Game finished!"+$"  You got {correctSongs.Count()} "+result;
                foreach (var song in SongList)
                {
                    song.ReInitialize();
                }
                await SongManager.SaveDataToXmlAsync(_correctSongs);
                return;
            }
            CoolDownStart();
        }

        private async void CountDownAnimation_Completed(object sender, object e)
        {           
            if (_gameStatus == true)
            {
              await GameStarted();
            }
            else
            {
                _ttlScore -= 100;
                SongPlayer.Stop();  
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetSongButton.Visibility = Visibility.Collapsed;
            SongList.Clear();           
           var songs = await SongManager.GetSongsAsync(AlbumsList);
            songs.ForEach(i => SongList.Add(i));
            CoolDownStart();
        }
    }
}
