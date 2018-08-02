using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TouchSwitch_Demo
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Create and Initialize
            TouchSwitch touchSwitch = new TouchSwitch(4);
            touchSwitch.Initialize();

            // Triggering when SwitchState changes
            touchSwitch.SwitchStateChanged += TouchSwitch_SwitchStateChanged;
        }

        private void TouchSwitch_SwitchStateChanged(object sender, SwitchStateChangedEventArgs e)
        {
            // TODO
            Debug.WriteLine($"Current Switch State : {e.SwitchState.ToString()}");
        }
    }
}
