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

namespace BMP180Demo
{
    public sealed partial class MainPage : Page
    {
        BMP180 sensor = new BMP180(Resolution.UltrHighResolution);

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await sensor.InitializeAsync();

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();   
        }

        private async void Timer_Tick(object sender, object e)
        {
            var data = await sensor.ReadAsync();
            Temp.Text = data.Temperature.ToString("0.00");
            Press.Text = data.Pressure.ToString("0.00");
            Alt.Text = data.Altitude.ToString("0.00");
        }
    }
}
