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

namespace ADXL345Demo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
         /* 
          * VCC - 3.3 V
          * GND -  GND
          * CS - CS0 - Pin24
          * SDO - Pin21
          * SDA - Pin19
          * SCL - Pin23
         */

        ADXL345 sensor = new ADXL345(0, GravityRange.Four);

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;            
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await sensor.InitializeAsync();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            string x, y, z;

            var accel = sensor.ReadAcceleration();
            x = String.Format("{0:F3}", accel.X);
            y = String.Format("{0:F3}", accel.Y);
            z = String.Format("{0:F3}", accel.Z);

            X.Text = x;
            Y.Text = y;
            Z.Text = z;
        }
    }
}
