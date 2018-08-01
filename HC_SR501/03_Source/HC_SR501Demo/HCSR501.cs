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
using Windows.Devices.Gpio;

namespace HC_SR501Demo
{
    class HCSR501 : IDisposable
    {
        private GpioPin sensor;
        private int pinOut;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pin">OUT Pin</param>
        public HCSR501(int pin)
        {
            pinOut = pin;
        }

        /// <summary>
        /// Initialize the sensor
        /// </summary>
        public void Initialize()
        {
            var gpio = GpioController.GetDefault();

            sensor = gpio.OpenPin(pinOut);

            sensor.SetDriveMode(GpioPinDriveMode.Input);
        }

        /// <summary>
        /// Read from the sensor
        /// </summary>
        /// <returns>Is Detected</returns>
        public bool Read()
        {
            if (sensor.Read() == GpioPinValue.High)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            sensor.Dispose();
        }
    }
}
