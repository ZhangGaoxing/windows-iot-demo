using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace ADS1115_Demo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //  Use ADX1115 to Read MQ Gas Sensor

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ADS1115 adc = new ADS1115(AddressSetting.GND, InputMultiplexeConfig.AIN0, PgaConfig.FS4096, DataRate.SPS860);
            await adc.InitializeAsync();

            while (true)
            {
                short raw = adc.ReadRaw();
                double vol = adc.RawToVoltage(raw);

                Debug.WriteLine($"Raw Data : {raw} => {vol} V");

                await Task.Delay(500);
            }
        }

    }
}
