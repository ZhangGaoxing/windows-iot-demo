using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace HC_SR04Demo
{
    class HCSR04
    {
        private int sensorTrig;
        private int sensorEcho;

        private GpioPin pinTrig;
        private GpioPin pinEcho;

        Stopwatch time = new Stopwatch();

        public HCSR04(int trig, int echo)
        {
            sensorTrig = trig;
            sensorEcho = echo;
        }

        public void Initialize()
        {
            var gpio = GpioController.GetDefault();

            pinTrig = gpio.OpenPin(sensorTrig);
            pinEcho = gpio.OpenPin(sensorEcho);

            pinTrig.SetDriveMode(GpioPinDriveMode.Output);
            pinEcho.SetDriveMode(GpioPinDriveMode.Input);

            pinTrig.Write(GpioPinValue.Low);
        }

        public async Task<double> ReadAsync()
        {
            double result;

            pinTrig.Write(GpioPinValue.High);
            await Task.Delay(10);
            pinTrig.Write(GpioPinValue.Low);

            while (pinEcho.Read() == GpioPinValue.Low)
            {

            }
            time.Restart();
            while (pinEcho.Read() == GpioPinValue.High)
            {

            }
            time.Stop();

            result = (time.Elapsed.TotalSeconds * 34000) / 2;

            return result;
        }

        public void Dispose()
        {
            pinTrig.Dispose();
            pinEcho.Dispose();
        }
    }
}
