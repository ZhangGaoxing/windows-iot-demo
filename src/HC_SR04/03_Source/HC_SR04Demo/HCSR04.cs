/*
 * MIT License
 * Copyright(c) 2018 - Zhang Yuexin

 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace HC_SR04Demo
{
    class HCSR04 : IDisposable
    {
        private int sensorTrig;
        private int sensorEcho;

        private GpioPin pinTrig;
        private GpioPin pinEcho;

        Stopwatch time = new Stopwatch();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="trig">Trig Pin</param>
        /// <param name="echo">Echo Pin</param>
        public HCSR04(int trig, int echo)
        {
            sensorTrig = trig;
            sensorEcho = echo;
        }

        /// <summary>
        /// Initialize the sensor
        /// </summary>
        public void Initialize()
        {
            var gpio = GpioController.GetDefault();

            pinTrig = gpio.OpenPin(sensorTrig);
            pinEcho = gpio.OpenPin(sensorEcho);

            pinTrig.SetDriveMode(GpioPinDriveMode.Output);
            pinEcho.SetDriveMode(GpioPinDriveMode.Input);

            pinTrig.Write(GpioPinValue.Low);
        }

        /// <summary>
        /// Read data from the sensor
        /// </summary>
        /// <returns>A double type distance data</returns>
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

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            pinTrig.Dispose();
            pinEcho.Dispose();
        }
    }
}
