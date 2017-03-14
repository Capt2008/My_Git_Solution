#region 程序集 System.Runtime.WindowsRuntime, Version=4.0.11.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// C:\Users\qicai21\.nuget\packages\System.Runtime.WindowsRuntime\4.0.11\ref\netcore50\System.Runtime.WindowsRuntime.dll
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using static System.Console;

namespace TestconsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
           

            #region 
            //var port = new Port();
            //WriteLine("Port is ready!");
            //for (int i = 0; i < 20; i++)
            //{
            //    Task.Delay(1000).Wait();
            //    port.Vessels[i].Sail();
            //    if (i==0)
            //    {
            //        Write("A vessel sailed.");
            //    }
            //    else 
            //    {
            //        Write("Another vessel sailed.");
            //    }
            //    WriteLine("Remaining vessels is {0}", port.VesselsOnBerth);
            //}
            //WriteLine("All vessel sailed, port is empty");
            #endregion

            #region 集合N选K
            //for (int j = 0; j < 10; j++)
            //{

            //    //随机从1-20中选出12个不重复的数字：
            //    var r = new Random();

            //    var result = new List<int>();
            //    var list = Enumerable.Range(1, 20).ToList();
            //    int k = 5;
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        if (list.Count-i<=k)
            //        {
            //            result.Add(list[i]);
            //            continue;
            //        }
            //        //对于a[0]，以k/n的概率选泽他。
            //        Task.Delay(100).Wait();
            //        int rt = r.Next(list.Count-i);
            //        if (rt<k)
            //        {
            //            result.Add(list[i]);
            //            k--;
            //        }                    
            //        //如果选中了，那么对剩下的数组来说，就是在n-1个元素中选k-1个元素的问题；
            //        //如果没选中，那么对剩下的数组来说，就是在n-1个元素中选k个元素的问题。迭代即可。

            //    }
            //    for (int i = 0; i < result.Count; i++)
            //    {
            //        Write(result[i] + ", ");
            //    }
            //    WriteLine(result.Count);
            //} 
            #endregion
        }

      
    }


    class Port
    {
        public Port()
        {
            VesselsOnBerth = 20;
            Vessels = new List<Vessel>();
            for (int i = 0; i < 20; i++)
            {
                var vsl = new Vessel() { port = this };
                Vessels.Add(vsl);
                //  vsl.VesselSailedEvent += OnVesselSailed;
            }
        }
        public int VesselsOnBerth { get; set; }
        public List<Vessel> Vessels { get; }

        private void OnVesselSailed(object sender, EventArgs e)
        {
            VesselsOnBerth--;
        }
    }


    class Vessel
    {
        public bool IsSailed { get; private set; } = false;
        public Port port { get; set; }
        public void Sail()
        {
            IsSailed = true;
            RaiseVslSailed();
            port.VesselsOnBerth--;
        }
        public event EventHandler VesselSailedEvent;
        public void RaiseVslSailed()
        {
            VesselSailedEvent?.Invoke(this, new EventArgs());
        }
    }
}
