using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace MyAlbumCoverMatchGame.Models
{
    internal class SongManager
    {
        //  负责从music library中把歌曲提取到xml中，
        //  xml加载歌曲到SongCollection，
        //  当xml不存在时创建xml
        //  每次app启动时异步校验曲库是否有更新
        //  将游戏中关于歌曲操作的结果记录到xml中
        //  其他
        private static string _xmlPath = ApplicationData.Current.LocalFolder.Path + @"\AlbumList0.xml";


        /// <summary>
        ///  check wether xml is available
        ///  ckeck music in xml is available
        ///  if not available, create a xml, run adding music to xml, ui suspend by processring with enent, send a message: first time run app, serializing music library, please wait!
        ///  if available, open dialog: user selecting how to play game: go on with last game or start a new game
        /// </summary>
        /// <returns></returns>
        internal static bool IsAppFirstRun()
        {

            if (File.Exists(_xmlPath))
            {
                return true;
            }
            else
            {
                //  xml is not available
                using (var stream = new FileStream(_xmlPath, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("AllAlbums"));
                    xdoc.Save(stream);
                }
                return false;
            }
        }

        internal async static Task RemoveNoAvailabeItem(bool startNewOrNot)
        {
            XDocument xdoc = XDocument.Load(_xmlPath);
            var albums = xdoc.Root.Elements("Album");
            var songs = albums.SelectMany(album => album.Elements());

            foreach (var song in songs)
            {
                await RemoveSongAsync(song, startNewOrNot);
            }

            using (var outstream = new FileStream(_xmlPath, FileMode.Create, FileAccess.Write))
            {
                xdoc.Save(outstream);
            }
        }

        /// <summary>
        /// 根据MusicLib更新xml，将xml中没有的歌曲添加进去
        /// 分三种情况：
        /// 1.xml为空，添加全部歌曲，Music.IsAvailable=true; 
        /// 2. 添加新增歌曲，Music.IsAvailable=true,其他不变；
        /// 3，添加新增歌曲，并将所有歌曲的Music.IsAvailable=true；
        /// </summary>
        /// <returns></returns>
        internal static async Task InitialXmlAsync()
        {
            var musicFiles = new List<StorageFile>();
            await GetAllSongs(KnownFolders.MusicLibrary, musicFiles);
            XDocument xdoc;
            using (var stream = new FileStream(_xmlPath, FileMode.Open, FileAccess.ReadWrite))
            {
                xdoc = XDocument.Load(stream);
            }
            var rootXel = xdoc.Root;
            //  rootXel所有的Song元素的Name=》List，提取fileName,如果List中不包含songName，
            //  找Album元素，找到了添加song,找不到，添加album,再添加song
            var songNameList = rootXel.Elements("Album").SelectMany(album => album.Elements("Song").Select(song => song.Attribute("Name").Value));
            foreach (var m in musicFiles)
            {
                var songProp = await m.Properties.GetMusicPropertiesAsync();

                if (!songNameList.Any(name => name == songProp.Title))  // Not contained
                {
                    var albumXel = rootXel.Elements("Album").FirstOrDefault(el => el.Attribute("Name").Value == songProp.Album);
                    if (albumXel == null)
                    {
                        rootXel.Add(new XElement("Album",
                            new XAttribute("Name", songProp.Album),
                            new XAttribute("AvailableSongs", "1"),
                            new XAttribute(await GetAlbumThumbnail(m)),
                            GetSongXElement(songProp, m.Path)));
                    }
                    else
                    {
                        albumXel.Add(GetSongXElement(songProp, m.Path));
                        var i = int.Parse(albumXel.Attribute("AvailableSongs").Value);
                        albumXel.Attribute("AvailableSongs").Value = (++i).ToString();
                    }
                }
            }
            using (var stream = new FileStream(_xmlPath, FileMode.Open, FileAccess.ReadWrite))
            {
                xdoc.Save(stream);
            }
        }

        private async static Task RemoveSongAsync(XElement song, bool startNewOrNot)
        {
            Task<bool> CheckSongExist = Task.Run(() => File.Exists(song.Attribute("Path").Value));
            if ((await CheckSongExist) != true)
            {
                //    如果album下只有这一首song，连同album一起删除
                if (song.Parent.Elements().Count() == 1)
                {
                    song.Parent.Remove();
                }
                else
                {
                    song.Remove();
                }
            }
            // ForStartNewGame
            if (startNewOrNot == true)
            {
                int n;
                if (song.Attribute("IsAvailable").Value != "true")
                {
                    song.Attribute("IsAvailable").Value = "true";
                    //    album.AvailableSongs+1
                    n = int.Parse(song.Parent.Attribute("AvailableSongs").Value);
                    song.Parent.Attribute("AvailableSongs").Value = (++n).ToString();
                }
            }
        }
        private async static Task<XAttribute> GetAlbumThumbnail(StorageFile musicFile)
        {
            //  Check Localcache 中是否已经有thumbmail
            var musicProps = await musicFile.Properties.GetMusicPropertiesAsync();
            string imageName = $"{musicProps.AlbumArtist}_{musicProps.Album}.jpg";
            var imageFile = await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync(imageName);
            if (!(imageFile == null))
            {
                return new XAttribute("Cover", imageFile.Path);
            }

            //  congmusic提取softwarebitmap
            SoftwareBitmap bitmap;
            using (var inStream = await musicFile.GetThumbnailAsync(ThumbnailMode.MusicView))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(inStream);
                bitmap = await decoder.GetSoftwareBitmapAsync();
            }

            //  保存softwarebitmap到LocalState中            
            StorageFile outputFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(imageName);
            using (var outstream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, outstream);
                encoder.SetSoftwareBitmap(bitmap);
                encoder.IsThumbnailGenerated = true;
                await encoder.FlushAsync();
            }

            //  把保存的thumbnail.path生成Attribute返回
            return new XAttribute("Cover", outputFile.Path);
        }

        private static XElement GetSongXElement(MusicProperties songProp, string songPath)
        {
            return new XElement("Song",
                                      new XAttribute("Name", songProp.Title),
                                      new XAttribute("Artist", songProp.Artist),
                                      new XAttribute("IsAvailable", "true"),
                                      new XAttribute("Path", songPath));
        }

        private static async Task GetAllSongs(StorageFolder musicLib, List<StorageFile> files)
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

        internal async static Task<List<Song>> GetSongsAsync(List<Album> albumsList)
        {
            Random r = new Random();
            var albums = from album in albumsList
                         where album.AvailableSongs > 0
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
                if (n - i == k)
                {
                    songs.Add(albums.ElementAt(i).GetSong());
                    continue;
                }
                await Task.Delay(10); // 防止伪随机数效应
                if (r.Next(n - i) < k)
                {
                    songs.Add(albums.ElementAt(i).GetSong());
                    k--;
                }
            }
            return songs;
        }

        internal static List<Album> GetAlbumDataSource()
        {
            XDocument albumsDoc = XDocument.Load(_xmlPath);
            XElement albums = albumsDoc.Root;
            var albumsList = new List<Album>();
            foreach (var el in albums.Elements("Album"))
            {
                Album album = new Album()
                {
                    Name = el.Attribute("Name").Value,
                    AvailableSongs = int.Parse(el.Attribute("AvailableSongs").Value),
                    ThumbnailPath = el.Attribute("Cover").Value
                };

                foreach (var songel in el.Elements("Song"))
                {
                    album.Allsongs.Add(
                        new Song(album)
                        {
                            UriStr = songel.Attribute("Path").Value,
                            Artist = songel.Attribute("Artist").Value,
                            Name = songel.Attribute("Name").Value,
                            IsAvailable = bool.Parse(songel.Attribute("IsAvailable").Value)
                        }
                       );
                }
                albumsList.Add(album);
            }
            return albumsList;
        }

        internal static async Task SaveDataToXmlAsync(List<Song> correctSongs)
        {
            List<string> correctSongNames = correctSongs.Select(s => s.Name).ToList();
            var xmlFile = await StorageFile.GetFileFromPathAsync(_xmlPath);
            XDocument xdoc;
            using (var stream = await xmlFile.OpenStreamForReadAsync())
            {
                xdoc = XDocument.Load(stream);
            }
            xdoc.Root.Elements("Album").ToList().ForEach(album =>
            {
                int i;
                foreach (var song in album.Elements())
                {
                    if (correctSongNames.Contains(song.Attribute("Name").Value))
                    {
                        song.Attribute("IsAvailable").Value = "false";
                        i = int.Parse(album.Attribute("AvailableSongs").Value);
                        album.Attribute("AvailableSongs").Value = (--i).ToString();
                    }
                }
            });
            using (var stream = await xmlFile.OpenStreamForWriteAsync())
            {
                xdoc.Save(stream);
            }
        }
    }
    internal class Album
    {
        internal string Name { get; set; }
        internal string ThumbnailPath { get; set; }
        internal int AvailableSongs { get; set; }
        internal List<Song> Allsongs { get; private set; } = new List<Song>();

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
