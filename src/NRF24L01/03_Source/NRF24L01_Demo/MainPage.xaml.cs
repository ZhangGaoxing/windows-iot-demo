using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace NRF24L01_Demo
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            NRF24L01 sender = new NRF24L01(0, 23, 24, "SPI0", 12);
            NRF24L01 receiver = new NRF24L01(0, 5, 6, "SPI1", 12);

            await sender.InitializeAsync();
            await receiver.InitializeAsync();

            sender.Send(Encoding.UTF8.GetBytes("ZhangGaoxing"));
            receiver.ReceivedData += Receiver_ReceivedData;
        }

        private void Receiver_ReceivedData(object sender, ReceivedDataEventArgs e)
        {
            var raw = e.Data.Skip(1).ToArray();
            var res = Encoding.UTF8.GetString(raw);

            Debug.Write("Received Raw Data : ");
            foreach (var item in raw)
            {
                Debug.Write($"{item} ");
            }
            Debug.WriteLine("");
            Debug.WriteLine($"Received Data : {res}");
        }
    }
}
