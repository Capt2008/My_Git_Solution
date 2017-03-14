using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace GetBitmapTest
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //  Get storagefile
            var files = await KnownFolders.MusicLibrary.GetFilesAsync();

            //  Exstract thumbnail image from file to type of softwarebitmap
            SoftwareBitmap sBitmap;
            using (var rStream = await files[0].GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.MusicView))
            {
                var decoder = await BitmapDecoder.CreateAsync(rStream);
                sBitmap = await decoder.GetSoftwareBitmapAsync();
            }

            #region 通过生成SoftewareBitmapSource来给UI的image控件提供数据源
            //  Check the soferwarebitmap property: pixelformat and alphamode
            //if (sBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || sBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            //{
            //    sBitmap = SoftwareBitmap.Convert(sBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            //}
            //  Show sofewarebitmap on UI
            //var imagesource = new SoftwareBitmapSource();
            //await imagesource.SetBitmapAsync(sBitmap);
            //MyImage.Source = imagesource; 
            #endregion

            //  Save to storagefile 
            StorageFile outfile = await ApplicationData.Current.LocalFolder.CreateFileAsync("coverimage_cache01.jpg");
            using (var outstream = await outfile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, outstream);
                encoder.SetSoftwareBitmap(sBitmap);
                encoder.IsThumbnailGenerated = true;
                await encoder.FlushAsync();
            }

            //  open a popup to show file saved
            //  var dialog = new MessageDialog("Image Saved successfully");
            //  await dialog.ShowAsync();
        }

        private async Task SaveWizSFTBPAsync(IRandomAccessStream randomStream)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(randomStream);
            SoftwareBitmap softBmp = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Testimage.jpg", CreationCollisionOption.GenerateUniqueName);
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                encoder.SetSoftwareBitmap(softBmp);
                await encoder.FlushAsync();
            }
        }
        /// <summary>
        /// 通过转成byte[]来实现图片储存，但不知道为什么就是不好用，得到得图片颜色配置不正确。
        /// 会不会因为方法将所有像素数据视为sRGB 颜色空间中的像素数据？
        /// </summary>
        /// <param name="thumbnail"></param>
        /// <returns></returns>
        private async Task SaveWizPixelAsync(StorageItemThumbnail thumbnail)
        {
            Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(thumbnail.GetInputStreamAt(0));
            byte[] buffer = new byte[thumbnail.Size];
            // byte[] pixelbytes;
            using (MemoryStream ms = new MemoryStream())
            {
                stream.Read(buffer, 0, buffer.Length);
            }
            #region 测试得到的buffer是否完整 结果是使用buffer生成的IrandomAccessTream来为BitmapImage.SetValue()提供参数，显示正常。
            //InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
            //DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
            //datawriter.WriteBytes(buffer);
            //await datawriter.StoreAsync();
            #endregion

            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Testimage.jpg", CreationCollisionOption.GenerateUniqueName);
            using (var sm = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, sm);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied
                    thumbnail.OriginalWidth,
                    thumbnail.OriginalHeight,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                   buffer);
                await encoder.FlushAsync();
            }
        }
    }
}
