using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.I2c;
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

namespace HMC5883L_Demo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        HMC5883L hmc5883l = null;

        DispatcherTimer timer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 0, 1)
        };

        public MainPage()
        {
            this.InitializeComponent();

            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, object e)
        {
            var data = hmc5883l.ReadRaw();
            string angle = hmc5883l.RawToDirectionAngle(data).ToString("f1");

            X.Text = data.X_Axis.ToString();
            Y.Text = data.Y_Axis.ToString();
            Z.Text = data.Z_Axis.ToString();
            Angle.Text = angle;

            Debug.WriteLine($"X-Axis:{data.X_Axis}, Y-Axis:{data.Y_Axis}, Z-Axis:{data.Z_Axis}");
            Debug.WriteLine($"Angle:{angle}");
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            hmc5883l = new HMC5883L(MeasurementMode.Continuous);
            await hmc5883l.InitializeAsync();

            timer.Start();
        }
    }
}
