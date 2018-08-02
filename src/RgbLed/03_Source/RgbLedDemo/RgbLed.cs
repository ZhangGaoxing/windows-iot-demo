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
using System.Threading.Tasks;
using Microsoft.IoT.Lightning.Providers;
using Windows.Devices.Pwm;
using Windows.UI;

namespace RgbLedDemo
{
    /// <summary>
    /// RGB LED Electrode Type
    /// </summary>
    public enum RgbType
    {
        /// <summary>
        /// Common Cathode
        /// </summary>
        CommonCathode,
        /// <summary>
        /// Common Anode
        /// </summary>
        CommonAnode
    }

    public class RgbLed : IDisposable
    {
        private PwmController controller;
        private double pwmFrequency;

        private PwmPin redPin;
        private PwmPin greenPin;
        private PwmPin bluePin;

        private int redPinNum;
        private int greenPinNum;
        private int bluePinNum;

        private RgbType type;

        /// <summary>
        /// Create A RgbLed Object
        /// </summary>
        /// <param name="redPin">Pin connect to red</param>
        /// <param name="greenPin">Pin connect to green</param>
        /// <param name="bluePin">Pin connect to blue</param>
        /// <param name="frequency">PWM Frequency</param>
        /// <param name="type">RGB LED Electrode Type</param>
        public RgbLed(int redPin, int greenPin, int bluePin, double frequency, RgbType type)
        {
            redPinNum = redPin;
            greenPinNum = greenPin;
            bluePinNum = bluePin;
            pwmFrequency = frequency;
            this.type = type;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public async Task InitializeAsync()
        {
            if (!LightningProvider.IsLightningEnabled)
            {
                throw new NullReferenceException("Lightning isn't enabled !");
            }

            controller = (await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider()))[1];
            controller.SetDesiredFrequency(pwmFrequency);

            redPin = controller.OpenPin(redPinNum);
            redPin.SetActiveDutyCyclePercentage(0);
            redPin.Start();          

            greenPin = controller.OpenPin(greenPinNum);
            greenPin.SetActiveDutyCyclePercentage(0);
            greenPin.Start();
            
            bluePin = controller.OpenPin(bluePinNum);
            bluePin.SetActiveDutyCyclePercentage(0);
            bluePin.Start();           
        }

        /// <summary>
        /// Set Red Pin Duty Cycle Percentage
        /// </summary>
        /// <param name="percentage">Percentage(From 0 to 1)</param>
        public void SetRedPin(double percentage)
        {
            if (type == RgbType.CommonCathode)
            {
                redPin.SetActiveDutyCyclePercentage(percentage);
            }
            else
            {
                redPin.SetActiveDutyCyclePercentage(1.0 - percentage);
            }
        }

        /// <summary>
        /// Set Green Pin Duty Cycle Percentage
        /// </summary>
        /// <param name="percentage">Percentage(From 0 to 1)</param>
        public void SetGreenPin(double percentage)
        {
            if (type == RgbType.CommonCathode)
            {
                greenPin.SetActiveDutyCyclePercentage(percentage);
            }
            else
            {
                greenPin.SetActiveDutyCyclePercentage(1.0 - percentage);
            }
        }

        /// <summary>
        /// Set Blue Pin Duty Cycle Percentage
        /// </summary>
        /// <param name="percentage">Percentage(From 0 to 1)</param>
        public void SetBluePin(double percentage)
        {
            if (type == RgbType.CommonCathode)
            {
                bluePin.SetActiveDutyCyclePercentage(percentage);
            }
            else
            {
                bluePin.SetActiveDutyCyclePercentage(1.0 - percentage);
            }
        }

        /// <summary>
        /// Show Color using RGB LED
        /// </summary>
        /// <param name="color">Color</param>
        public void ShowColor(Color color)
        {
            if (type == RgbType.CommonCathode)
            {
                redPin.SetActiveDutyCyclePercentage(color.R / 255.0);
                greenPin.SetActiveDutyCyclePercentage(color.G / 255.0);
                bluePin.SetActiveDutyCyclePercentage(color.B / 255.0);
            }
            else
            {
                redPin.SetActiveDutyCyclePercentage(1.0 - color.R / 255.0);
                greenPin.SetActiveDutyCyclePercentage(1.0 - color.G / 255.0);
                bluePin.SetActiveDutyCyclePercentage(1.0 - color.B / 255.0);
            }
        }

        /// <summary>
        /// Breathing LED
        /// </summary>
        /// <param name="delay">Delay Time</param>
        public async Task BreathingAsync(int delay)
        {
            if (type == RgbType.CommonCathode)
            {
                double red = 255;
                double green = 0;
                double blue = 0;

                while (red != 0 && green != 255)
                {
                    redPin.SetActiveDutyCyclePercentage(red / 255.0);
                    greenPin.SetActiveDutyCyclePercentage(green / 255.0);

                    //red = red - 12.75;
                    //green = green + 12.75;
                    red--;
                    green++;
                    await Task.Delay(delay);
                }

                while (green != 0 && blue != 255)
                {
                    greenPin.SetActiveDutyCyclePercentage(green / 255.0);
                    bluePin.SetActiveDutyCyclePercentage(blue / 255.0);

                    //green = green - 12.75;
                    //blue = blue + 12.75;
                    green--;
                    blue++;
                    await Task.Delay(delay);
                }

                while (blue != 0 && red != 255)
                {
                    bluePin.SetActiveDutyCyclePercentage(blue / 255.0);
                    redPin.SetActiveDutyCyclePercentage(red / 255.0);

                    //blue = blue - 12.75;
                    //red = red + 12.75;
                    blue--;
                    red++;
                    await Task.Delay(delay);
                }
            }
            else
            {
                double red = 0;
                double green = 255;
                double blue = 255;

                while (red != 255 && green != 0)
                {
                    redPin.SetActiveDutyCyclePercentage(red / 255.0);
                    greenPin.SetActiveDutyCyclePercentage(green / 255.0);

                    red++;
                    green--;
                    await Task.Delay(delay);
                }

                while (green != 255 && blue != 0)
                {
                    greenPin.SetActiveDutyCyclePercentage(green / 255.0);
                    bluePin.SetActiveDutyCyclePercentage(blue / 255.0);

                    green++;
                    blue--;
                    await Task.Delay(delay);
                }

                while (blue != 255 && red != 0)
                {
                    bluePin.SetActiveDutyCyclePercentage(blue / 255.0);
                    redPin.SetActiveDutyCyclePercentage(red / 255.0);

                    blue++;
                    red--;
                    await Task.Delay(delay);
                }
            }
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            redPin.Stop();
            redPin.Dispose();

            greenPin.Stop();
            greenPin.Dispose();

            bluePin.Stop();
            bluePin.Dispose();
        }
    }
}
