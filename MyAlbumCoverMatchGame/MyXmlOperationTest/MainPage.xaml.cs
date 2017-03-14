using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MyXmlOperationTest
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<Berth> BerthsList { get; set; } = new ObservableCollection<Berth>();
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var path = ApplicationData.Current.LocalFolder.Path + @"\testData.xml";
            //   检索xml,.找到port节点
            XElement portElements;
            Port port;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                portElements = XElement.Load(stream);
            }
            var targetElement = portElements.Elements().FirstOrDefault(p => p.Attribute("Name").Value == PortTextBox.Text);

            if (targetElement == null)
            {
                port = new Port() { Name = PortTextBox.Text };
                targetElement = new XElement("Port",
                    new XAttribute("Name", port.Name),
                    new XAttribute("City", string.Empty)
                    );
                portElements.Add(targetElement);                
            }
           
            else
            {
                port = BerthsList.Select(b => b.Port).First(p => p.Name == PortTextBox.Text);
            }
            var newBerth=GetNewBerth(port);
            BerthsList.Add(newBerth);
            AddBerthToPort(targetElement, newBerth);
            using (var stream = await Task.Run(() => new FileStream(ApplicationData.Current.LocalFolder.Path + @"\testData.xml", FileMode.OpenOrCreate, FileAccess.Write)))
            {
                portElements.Save(stream);
            }
            var dialog = new MessageDialog("Save successed!");
            await dialog.ShowAsync();
            FlushInputLable();         
        }
        /// <summary>
        /// Add newBerth to target port, flush xml file
        /// </summary>
        /// <param name="portElement"></param>
        /// <param name="newBerth"></param>
        /// <param name="path"></param>
        public void AddBerthToPort(XElement portElement, Berth newBerth)
        {
            //  Create new Berth element
            var berthElement = new XElement("Berth",
                                new XAttribute("No", newBerth.Name),
                                new XAttribute("IsSeparable", "false"),
                                new XAttribute("Length", newBerth.Length.ToString()),
                                new XAttribute("MaxDepth", newBerth.MaxDepth.ToString()),
                                new XAttribute("Capacity", newBerth.Capacity)
                );
            portElement.Add(berthElement);
        }

        public void WriteByWriter(XElement rootXElement,string path)
        {
            
        }

        private Berth GetNewBerth(Port port)
        {
            var newBerth = new Berth()
            {
                Name = NameTextBox.Text,
                Port = port,
                Length = double.Parse(LengthTextBox.Text),
                Capacity = CapacityTextBox.Text,
                MaxDepth = double.Parse(DepthTextBox.Text)
            };
            port.Berths.Add(newBerth);
            return newBerth;
        }
        private void FlushInputLable()
        {
            NameTextBox.Text = "";
            PortTextBox.Text = "";
            LengthTextBox.Text = "";
            DepthTextBox.Text = "";
            CapacityTextBox.Text = "";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var portsList = BerthsList.GroupBy(berth => berth.Port).Select(i => i.Key);
            XElement dataTree = new XElement("ports");
            foreach (var item in portsList)
            {
                // 添加port
                var portElement = new XElement(
                    new XElement(item.GetType().Name,
                    new XAttribute("Name", item.Name),
                    new XAttribute("City", string.IsNullOrEmpty(item.City) ? string.Empty : item.City)
                    ));
                dataTree.Add(portElement);
                //  添加berth
                foreach (var berth in item.Berths)
                {
                    var berthElement = new XElement(berth.GetType().Name,
                        new XAttribute("No", berth.Name)
                        );
                    GetBerthRecursive(berth, berthElement);
                    portElement.Add(berthElement);
                }
            }


            XDocument xdoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment($"This xml was built by MyApps, at {DateTime.Now.TimeOfDay}."),
                dataTree
                );
            using (FileStream stream = new FileStream(ApplicationData.Current.LocalFolder.Path + @"\testData.xml", FileMode.Create, FileAccess.Write))
            {
                xdoc.Save(stream);
            }
        }

        private void GetBerthRecursive(Berth berth, XElement berthElement)
        {
            if (berth.SubBerth.Count > 0)
            {
                berthElement.SetAttributeValue("IsSeparable", "true");
                foreach (var subBerth in berth.SubBerth)
                {
                    var subElement = new XElement(subBerth.GetType().Name,
                        new XAttribute("No", subBerth.Name)
                        );
                    GetBerthRecursive(subBerth, subElement);
                    berthElement.Add(subElement);
                }
            }
            else
            {
                berthElement.SetAttributeValue("IsSeparable", "false");
                berthElement.SetAttributeValue("Length", berth.Length);
                berthElement.SetAttributeValue("MaxDepth", berth.MaxDepth);
                berthElement.SetAttributeValue("Capacity", berth.Capacity);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            XElement xels;
            using (var buffer = new FileStream(ApplicationData.Current.LocalFolder.Path + @"\testData.xml", FileMode.Open, FileAccess.Read))
            {
                xels = XElement.Load(buffer);
            }

            foreach (var portitem in xels.Elements())
            {
                var port = new Port()
                {
                    Name = portitem.FirstAttribute.Value,
                    City = portitem.LastAttribute.Value
                };
                foreach (var berthitem in portitem.Elements())
                {
                    if (berthitem.Attribute("IsSeparable").Value == "true")
                    {
                        var berth = new Berth()
                        {
                            Name = berthitem.Attribute("No").Value,
                            Port = port
                        };
                        foreach (var item in berthitem.Elements())
                        {
                            berth.SubBerth.Add(new Berth()
                            {
                                Name = item.Attribute("No").Value,
                                Length = double.Parse(item.Attribute("Length").Value),
                                MaxDepth = double.Parse(item.Attribute("MaxDepth").Value),
                                Capacity = item.Attribute("Capacity").Value,
                                Port = port
                            });
                        }
                        port.Berths.Add(berth);
                    }
                    else
                    {
                        port.Berths.Add(new Berth()
                        {
                            Name = berthitem.Attribute("No").Value,
                            Length = double.Parse(berthitem.Attribute("Length").Value),
                            MaxDepth = double.Parse(berthitem.Attribute("MaxDepth").Value),
                            Capacity = berthitem.Attribute("Capacity").Value,
                            Port = port
                        });
                    }
                }
                port.Berths.ForEach(i => BerthsList.Add(i));
            }
        }

        private async void WriterButton_Click(object sender, RoutedEventArgs e)
        {
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var writer = xdoc.CreateWriter();
            var path = ApplicationData.Current.LocalFolder.Path + @"\Data.xml";
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await TestWriter(stream);
            }
            
        }

        async Task TestWriter(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Async = true;

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartElement("Person");
                writer.WriteAttributeString("Name", "peter");              
                await writer.WriteEndElementAsync();
                await writer.FlushAsync();
            }
        }
    }


}
