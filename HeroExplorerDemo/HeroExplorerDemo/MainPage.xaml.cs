using HeroExplorerDemo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace HeroExplorerDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<Character> CharacterList;
        public MainPage()
        {
            this.InitializeComponent();
            CharacterList = new ObservableCollection<Character>();
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            CharacterList.Clear();
            var list = await MarvelFacade.GetCharactersAsync(10, 100);
            list.ForEach(i => CharacterList.Add(i));
        }
    }

    public class ThumbnailConvertor:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Thumbnail thumbnail = value as Thumbnail;
            BitmapImage avator = new BitmapImage(new Uri($"{thumbnail.path}.{thumbnail.extension}"));
            return avator;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
