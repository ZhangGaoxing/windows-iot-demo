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

namespace ULN2003A_Demo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ULN2003A uln2003a = null;

        // 28BYJ-48
        const double _stepAngle = 0.08789;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            uln2003a = new ULN2003A(26, 13, 6, 5);

            TurnDirection direction = (TurnDirection)Enum.Parse(typeof(TurnDirection), Direction.SelectionBoxItem.ToString());
            DrivingMethod method = (DrivingMethod)Enum.Parse(typeof(DrivingMethod), Method.SelectionBoxItem.ToString());

            await uln2003a.TurnAsync(int.Parse(Degree.Text), _stepAngle, direction, method);
        }

        private async void Start1_Click(object sender, RoutedEventArgs e)
        {
            uln2003a = new ULN2003A(26, 13, 6, 5);

            TurnDirection direction = (TurnDirection)Enum.Parse(typeof(TurnDirection), Direction.SelectionBoxItem.ToString());
            DrivingMethod method = (DrivingMethod)Enum.Parse(typeof(DrivingMethod), Method.SelectionBoxItem.ToString());

            await uln2003a.TurnAsync(_stepAngle, direction, method);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            uln2003a.Dispose();
        }
    }
}
