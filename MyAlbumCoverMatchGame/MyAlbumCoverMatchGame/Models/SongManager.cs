using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace MyAlbumCoverMatchGame.Models
{
    internal class SongManager
    {
        internal SongManager()
        {
            AlbumList = new ObservableCollection<Album>();
            InitializeAsync();
        }

        internal ObservableCollection<Album> AlbumList { get; set; }

        internal async void InitializeAsync()
        {
            var songList = new List<StorageFile>();
            await GetAllSongs(KnownFolders.MusicLibrary, songList);
            foreach (var song in songList)
            {
                var songData = await song.Properties.GetMusicPropertiesAsync();
                try
                {
                    var album = AlbumList.First(i => { return i.Name == songData.Album; });
                    album.AddSong(new Song(album) { UriStr = song.Path });
                }
                catch (Exception)
                {
                    var album = new Album(song, songData);
                    album.AddSong(new Song(album) { UriStr = song.Path });
                    AlbumList.Add(album);
                }
            }
        }

        private async Task GetAllSongs(StorageFolder musicLib, List<StorageFile> files)
        {
            var task = musicLib.GetFoldersAsync().AsTask();
            var list = await musicLib.GetFilesAsync();
            foreach (var itemfile in list)
            {
                if (itemfile.ContentType == "audio/x-wav" || itemfile.ContentType == "audio/mpeg")
                {
                    files.Add(itemfile);
                }
            }
            var folders = await task;
            foreach (var itemfolder in folders)
            {
                await GetAllSongs(itemfolder, files);
            }
        }

        internal async Task<List<Song>> GetSongsAsync()
        {
            Random r = new Random();
            var albums = from album in AlbumList
                         where album.AvailableSongs != 0
                         select album;

            //  Check AvailableAlumb是否足够；
            if (albums.Count() < 10)
            {
                return null;
            }

            var songs = new List<Song>();
            int n = albums.Count();
            int k = 10;
            for (int i = 0; i < n; i++)
            {
                if (n - i <= k)
                {
                    songs.Add(albums.ElementAt(i).GetSong());
                    continue;
                }
               await Task.Delay(10);
                if (r.Next(n - i) < k)
                {
                    songs.Add(albums.ElementAt(i).GetSong());
                    k--;
                }
            }
            return songs;
        }
    }

    internal class Album
    {
        internal Album()
        {

        }
        internal Album(StorageFile songFile, MusicProperties songProperties)
        {
            Name = songProperties.Album;
            GetThumbnailAsync(songFile);
        }

        internal string Name { get; set; }
        internal BitmapImage Thumbnail { get; set; } = new BitmapImage();
        internal int AvailableSongs { get; set; }

        internal List<Song> Allsongs { get; private set; } = new List<Song>();

        internal void AddSong(Song song)
        {
            Allsongs.Add(song);
            AvailableSongs++;
        }

        private async void GetThumbnailAsync(StorageFile songFile)
        {
            var stream = await songFile.GetThumbnailAsync(ThumbnailMode.MusicView);
            await Thumbnail.SetSourceAsync(stream);
        }

        internal Song GetSong()
        {
            var songs = from song in Allsongs
                        where song.IsAvailable == true
                        select song;
            if (songs.Count() == 1) return songs.First();
            var r = new Random();
            return songs.ElementAt(r.Next(songs.Count()));
        }
    }
}
