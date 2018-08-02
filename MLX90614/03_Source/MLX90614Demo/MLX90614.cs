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
using Windows.Devices.I2c;

namespace MLX90614Demo
{
    public struct MLX90614Data
    {
        public double AmbientTemp;
        public double ObjectTemp;
    }

    public class MLX90614 : IDisposable
    {
        private I2cDevice sensor;

        private const byte MLX90614_ADDR = 0x5A;
        private const byte MLX90614_AMBIENT_TEMP = 0x06;
        private const byte MLX90614_OBJECT_TEMP = 0x07;

        /// <summary>
        /// Initialize
        /// </summary>
        public async Task InitializeAsync()
        {
            var settings = new I2cConnectionSettings(MLX90614_ADDR);
            settings.BusSpeed = I2cBusSpeed.StandardMode;

            var controller = await I2cController.GetDefaultAsync();
            sensor = controller.GetDevice(settings);
        }

        /// <summary>
        /// Read Seneor Data
        /// </summary>
        public MLX90614Data Read()
        {
            byte[] readBuf = new byte[2];
            MLX90614Data data = new MLX90614Data();

            sensor.WriteRead(new byte[] { MLX90614_AMBIENT_TEMP }, readBuf);
            data.AmbientTemp = BitConverter.ToInt16(readBuf, 0) * 0.02 - 273.15;
            sensor.WriteRead(new byte[] { MLX90614_OBJECT_TEMP }, readBuf);
            data.ObjectTemp = BitConverter.ToInt16(readBuf, 0) * 0.02 - 273.15;

            return data;
        }

        /// <summary>
        /// Get MLX90614 Device
        /// </summary>
        /// <returns></returns>
        public I2cDevice GetDevice()
        {
            return sensor;
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
