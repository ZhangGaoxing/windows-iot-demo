using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MAX7219_Demo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MAX7219 led = new MAX7219(0);
        // Love Pattern
        byte[] ch = { 0x66, 0xE7, 0xFF, 0xFF, 0xFF, 0x7E, 0x3C, 0x18 };

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {           
            await led.InitializeAsync();

            led.SetDecode(DecodeMode.NoDecode);
            led.SetIntensity(0);

            for (int i = 0; i < 8; i++)
            {
                led.SetRow(i, ch[i]);
            }
        }
    }
}
