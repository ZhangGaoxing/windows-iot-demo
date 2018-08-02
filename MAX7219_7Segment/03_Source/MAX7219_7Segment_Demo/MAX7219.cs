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
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;

namespace MAX7219_7Segment_Demo
{
    /// <summary>
    /// Set MAX7219 Power Mode
    /// </summary>
    enum PowerMode
    {
        Shutdown = 0x00,
        Normal = 0x01        
    }

    /// <summary>
    /// MAX7219 Test Mode
    /// </summary>
    enum DisplayTestMode
    {
        Normal = 0x00,
        Test = 0x01
    }

    /// <summary>
    /// MAX7219 Decode Mode
    /// </summary>
    enum DecodeMode
    {
        /// <summary>
        /// When you drive 8x8 display, select this.
        /// </summary>
        NoDecode = 0x00,
        /// <summary>
        /// Just decode register data D0.
        /// </summary>
        Digit0 = 0x01,
        /// <summary>
        /// Decode register data D3-D0
        /// </summary>
        Digit3 = 0x0F,
        /// <summary>
        /// Decode register Data D7-D0
        /// </summary>
        Digit7 = 0xFF
    }

    class MAX7219 : IDisposable
    {
        #region
        const byte MAX_DIG0_REG_ADDR = 0x01;
        const byte MAX_DIG1_REG_ADDR = 0x02;
        const byte MAX_DIG2_REG_ADDR = 0x03;
        const byte MAX_DIG3_REG_ADDR = 0x04;
        const byte MAX_DIG4_REG_ADDR = 0x05;
        const byte MAX_DIG5_REG_ADDR = 0x06;
        const byte MAX_DIG6_REG_ADDR = 0x07;
        const byte MAX_DIG7_REG_ADDR = 0x08;
        readonly byte[] MAX_DIG_REG_ADDR = { MAX_DIG0_REG_ADDR, MAX_DIG1_REG_ADDR, MAX_DIG2_REG_ADDR, MAX_DIG3_REG_ADDR, MAX_DIG4_REG_ADDR, MAX_DIG5_REG_ADDR, MAX_DIG6_REG_ADDR, MAX_DIG7_REG_ADDR };

        const byte MAX_DECODE_REG_ADDR = 0x09;
        const byte MAX_INTENSITY_REG_ADDR = 0x0A;
        const byte MAX_SCAN_REG_ADDR = 0x0B;
        const byte MAX_POWER_REG_ADDR = 0x0C;
        const byte MAX_TEST_REG_ADDR = 0x0F;
        #endregion

        private SpiDevice sensor;

        private int chipSelect;
        private DecodeMode decodeMode;

        public MAX7219(int chipSelect, DecodeMode decodeMode)
        {
            this.chipSelect = chipSelect;
            this.decodeMode = decodeMode;
        }

        /// <summary>
        /// Initialize MAX7219
        /// </summary>
        public async Task InitializeAsync()
        {
            var settings = new SpiConnectionSettings(chipSelect);
            settings.ClockFrequency = 5000000;
            settings.Mode = SpiMode.Mode3;
            settings.SharingMode = SpiSharingMode.Shared;

            string aqs = SpiDevice.GetDeviceSelector();
            var dis = await DeviceInformation.FindAllAsync(aqs);
            sensor = await SpiDevice.FromIdAsync(dis[0].Id, settings);

            sensor.Write(new byte[] { MAX_SCAN_REG_ADDR, 0x07 });
            SetPower(PowerMode.Normal);
            SetDecode(decodeMode);
            Clear();
        }

        /// <summary>
        /// Set Register Data and Print
        /// </summary>
        /// <param name="index">Segment index, range from 0 to 7</param>
        /// <param name="val">Printed data</param>
        /// <param name="isDecimal">Does it show Decimal Point (Only for DecodeMode is Digit7)</param>
        public void SetSegment(int index, byte value, bool isDecimal = false)
        {
            byte val = value;
            if (isDecimal)
            {
                val = (byte)(val | 0x80);
            }

            sensor.Write(new byte[] { MAX_DIG_REG_ADDR[index], val });
        }

        /// <summary>
        /// Set MAX7219 Decode Mode
        /// </summary>
        /// <param name="decodeMode">Mode</param>
        public void SetDecode(DecodeMode decodeMode)
        {
            this.decodeMode = decodeMode;
            sensor.Write(new byte[] { MAX_DECODE_REG_ADDR, (byte)decodeMode });
        }

        /// <summary>
        /// Set Brightness
        /// </summary>
        /// <param name="val">In range 0-15</param>
        public void SetIntensity(int val)
        {
            if (val >= 0 && val <=15)
            {
                sensor.Write(new byte[] { MAX_INTENSITY_REG_ADDR, (byte)val });
            }
            else
            {
                throw new ArgumentOutOfRangeException("val", "Value out of range.");
            }
        }

        /// <summary>
        /// Test Display
        /// </summary>
        /// <param name="mode">Mode</param>
        public void DisplayTest(DisplayTestMode mode)
        {
            sensor.Write(new byte[] { MAX_TEST_REG_ADDR, (byte)mode });
        }

        /// <summary>
        /// Set MAX7219 Power
        /// </summary>
        /// <param name="mode">Mode</param>
        public void SetPower(PowerMode mode)
        {
            sensor.Write(new byte[] { MAX_POWER_REG_ADDR, (byte)mode });
        }

        /// <summary>
        /// Clear the Segment
        /// </summary>
        public void Clear()
        {
            byte clearVal = 0x00;
            if (decodeMode != DecodeMode.NoDecode)
            {
                clearVal = 0x0F;
            }

            for (int i = 0; i < 8; i++)
            {
                sensor.Write(new byte[] { MAX_DIG_REG_ADDR[i], clearVal });
            }
        }

        /// <summary>
        /// Get MAX7219 Device
        /// </summary>
        public SpiDevice GetDevice()
        {
            return sensor;
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            SetPower(PowerMode.Shutdown);
            sensor.Dispose();
        }
    }
}
