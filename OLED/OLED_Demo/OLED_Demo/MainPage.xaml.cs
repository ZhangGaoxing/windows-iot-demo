using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace OLED_Demo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        OLED oled = new OLED();
        CharactersTable table = new CharactersTable();

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await oled.InitializeAsync();
            
            oled.ShowChar(80, 48 / 8, 16, 16, table.Zhang);
            oled.ShowChar(96, 48 / 8, 16, 16, table.Gao);
            oled.ShowChar(112, 48 / 8, 16, 16, table.Xing);

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");

            for (int i = 0; i < time.Length; i++)
            {
                switch (time[i])
                {
                    case '0':
                        oled.ShowChar(0 + 8 * i, 0, 8, 16, table.Zero);
                        break;
                    case '1':
                        oled.ShowChar(0 + 8 * i, 0, 8, 16, table.One);
                        break;
                    case '2':
                        oled.ShowChar(0 + 8 * i, 0, 8, 16, table.Two);
                        break;
                    case '3':
                        oled.ShowChar(0 + 8 * i, 0, 8, 16, table.Three);
                        break;
                    case '4':
                        oled.ShowChar(0 + 8 * i, 0, 8, 16, table.Four);
                        break;
                    case '5':
                        oled.ShowChar(0 + 8 * i, 0, 8, 16, table.Five);
                        break;
                    case '6':
                        oled.ShowChar(0 + 8 * i, 0, 8, 16, table.Six);
                        break;
                    case '7':
                        oled.ShowChar(0 + 8 * i, 0, 8, 16, table.Seven);
                        break;
                    case '8':
                        oled.ShowChar(0 + 8 * i, 0, 8, 16, table.Eight);
                        break;
                    case '9':
                        oled.ShowChar(0 + 8 * i, 0, 8, 16, table.Nine);
                        break;
                    case ':':
                        oled.ShowChar(0 + 8 * i, 0, 8, 16, table.Colon);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
