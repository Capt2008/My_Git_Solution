using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace MyAlbumCoverMatchGame.Models
{
    internal class Song
    {
        internal Song(Album album)
        {
            _isAvailable = true;
            Album = album;
        }

        internal string UriStr { get; set; }
        internal Album Album { get; set; }
        internal BitmapImage Thumbnail => Album.Thumbnail;
        internal bool IsAvailable
        {
            get { return _isAvailable; }
            set
            {
                _isAvailable = value;
                Album.AvailableSongs--;
            }
        }

        private bool _isAvailable;
    }
}
