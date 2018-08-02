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

namespace TouchSwitch_Demo
{
    /// <summary>
    /// Switch State
    /// </summary>
    enum SwitchState
    {
        OFF,
        ON,
    }

    class SwitchStateChangedEventArgs : EventArgs
    {
        public readonly SwitchState SwitchState;
        public SwitchStateChangedEventArgs(SwitchState state)
        {
            SwitchState = state;
        }
    }

    class TouchSwitch : IDisposable
    {
        private GpioPin sensor;
        private int pin;

        /// <summary>
        /// Get Switch State
        /// </summary>
        public SwitchState State
        {
            get
            {
                return ReadState();
            }
        }

        /// <summary>
        /// Create a TouchSwitch object
        /// </summary>
        /// <param name="pin">Pin number</param>
        public TouchSwitch(int pin)
        {
            this.pin = pin;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            var gpio = GpioController.GetDefault();
            sensor = gpio.OpenPin(pin);
            sensor.SetDriveMode(GpioPinDriveMode.Input);

            sensor.ValueChanged += Sensor_ValueChanged;
        }

        /// <summary>
        /// Read GpioPinValue
        /// </summary>
        /// <returns>GpioPinValue</returns>
        public GpioPinValue Read()
        {
            return sensor.Read();
        }

        /// <summary>
        /// Read SwitchState
        /// </summary>
        /// <returns>SwitchState</returns>
        private SwitchState ReadState()
        {
            var value = sensor.Read();
            switch (value)
            {
                case GpioPinValue.Low:
                    return SwitchState.OFF;
                case GpioPinValue.High:
                    return SwitchState.ON;
                default:
                    return SwitchState.OFF;
            }
        }
        
        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            sensor.Dispose();
        }

        public delegate void SwitchStateChangedHandle(object sender, SwitchStateChangedEventArgs e);
        /// <summary>
        /// Triggering when SwitchState changes
        /// </summary>
        public event SwitchStateChangedHandle SwitchStateChanged;

        private void Sensor_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            SwitchStateChanged(sender, new SwitchStateChangedEventArgs(ReadState()));
        }
    }
}
