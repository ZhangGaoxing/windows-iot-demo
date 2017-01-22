using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace BH1750FVIDemo
{
    public sealed partial class MainPage : Page
    {
        BH1750FVI sensor = new BH1750FVI(AddressSetting.AddPinLow, MeasurementMode.ContinuouslyHighResolutionMode, LightTransmittance.Hundred);

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {           
            await sensor.InitializeAsync();
            
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            Data.Text = sensor.Read().ToString("0.00");
        }
    }
}
