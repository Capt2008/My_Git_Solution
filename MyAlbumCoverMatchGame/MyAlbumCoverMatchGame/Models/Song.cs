using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using static MyAlbumCoverMatchGame.Models.SongManager;

namespace MyAlbumCoverMatchGame.Models
{
    internal class Song:INotifyPropertyChanged
    {
        internal Song(Album album)
        {            
            Album = album;
            _thumbnail = GetBitmapSource();
        }

        internal bool? GameMark { get; set; } = null;
        internal string Name { get; set; }
        internal string Artist { get; set; }
        internal string UriStr { get; set; }
        internal Album Album { get; set; }       
        internal BitmapImage Thumbnail
        { get
            { return _thumbnail; }
            set
            {
                _thumbnail = value;
                RaisedPropertyChanged();
            }
        }            
        internal bool IsAvailable
        {
            get { return _isAvailable; }
            set
            {
                _isAvailable = value;                
            }
        }

        internal void AfterPlayed()
        {
            _isAvailable = false;
            Album.AvailableSongs--;
        }

        private BitmapImage GetBitmapSource()
        {
            return new BitmapImage(new Uri(Album.ThumbnailPath));
        }
        

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisedPropertyChanged([CallerMemberName]string name="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void ReInitialize()
        {
            _thumbnail = GetBitmapSource();
            GameMark = null;
        }

        private bool _isAvailable;
        private BitmapImage _thumbnail;
    }
}
