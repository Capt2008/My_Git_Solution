using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace TestForBitmapChanged
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<TestObj> List;
        public MainPage()
        {
            this.InitializeComponent();
            GetList();
        }

        private void GetList()
        {
            List = new ObservableCollection<TestObj>();
            for (int i = 0; i < 5; i++)
            {
                List.Add(new TestObj(i));
            }
        }
               

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            List[0].Name = "haha";
           // List[0].ChangedName();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
             List[0].Avator = new BitmapImage(new Uri("ms-appx:///Assets/correct.png"));
          //  List[0].ChangedAvator();
        }
    }



    public class TestObj:INotifyPropertyChanged
    {
        public TestObj(int i)
        {
            Name = "Str" + i;
            Avator = new BitmapImage(new Uri("ms-appx:///Assets/incorrect.png"));
        }

        string _name;
        BitmapImage _avator;
        public BitmapImage Avator
        {
            get { return _avator; }
            set { _avator = value; RaisedPropertyChanged("Avator"); }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisedPropertyChanged("Name"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisedPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void ChangedName()
        {
            Name = "haha";
            RaisedPropertyChanged("Name");
        }

        public void ChangedAvator()
        {
            Avator = new BitmapImage(new Uri("ms-appx:///Assets/correct.png"));
            RaisedPropertyChanged("Avator");
        }
    }
}
