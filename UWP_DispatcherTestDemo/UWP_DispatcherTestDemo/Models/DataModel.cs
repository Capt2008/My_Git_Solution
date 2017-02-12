using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace UWP_DispatcherTestDemo.Models
{
    public class BindingDataBase : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class DataModel : BindingDataBase
    {
        //  要给前台提供一个个或多个绑定属性 Supplying one or more properties for UI binding
        //  有方法对属性进行异步处理 Using Asynchronous method to do the changing binding property
        //  Using IsBusying toggle for ProgressRing at foreUI
        //  Using CancellationTokenSource for Cancel Asynchronous Method operation
        //  因为有时异步操作的需求来自于类内部，比如自有的异步方法；
        //  有时异步需求是类本身调用一个同步方法，但耗时比较严重比如从Server上download Image

        string _content;
        public string Content
        {
            get => _content;
            private set
            {
                _content = value;
                RaisePropertyChanged();
            }
        }

        bool _isBusying;
        public bool IsBusying
        {
            get { return _isBusying; }
            private set
            {
                _isBusying = value;
                RaisePropertyChanged();
            }
        }

        //  come in handy
        //  private CancellationTokenSource CTS { get; set; }

        public async Task SetContentAsync(string str)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
              {
                  IsBusying = true;
                  await Task.Delay(3500);
                  IsBusying = false;
                  Content = str;
              });
        }

        public async Task JumpinQueue()
        {
            await new Windows.UI.Xaml.Controls.UserControl().Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
             {
                 await Task.Delay(3000);
                 Content = "我是插队进来的";
             });
        }

        public void BlockThread(string str)
        {
            IsBusying = true;
            Task.Delay(4000);
            IsBusying = false;
            Content = str;
        }

    }
}
