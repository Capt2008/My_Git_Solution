﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace UWP_DispatcherTestDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Models.DataModel Model { get; set; } = new Models.DataModel();
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        { 
            await Model.SetContentAsync(string.IsNullOrEmpty(MyTextBox.Text) ? "Sorry, TextBox is empty!" : MyTextBox.Text, 2000);
           
            //  会抛出Exception from HRESULT: 0x8001010E
            //  await Task.Run(() => { new BitmapImage(); });

            //  通过CancellationTokenSource设置异步超时取消
            //  var cts = new CancellationTokenSource();
            //  cts.CancelAfter(2000);            

            //  await Model.JumpinQueue();

            // Model.BlockThread(string.IsNullOrEmpty(MyTextBox.Text) ? "Sorry, TextBox is empty!" : MyTextBox.Text);
        }
    }
}
