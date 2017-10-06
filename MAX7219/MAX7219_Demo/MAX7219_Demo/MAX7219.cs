using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;

namespace MAX7219_Demo
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

        private SpiDevice led;

        private int chipSelect;

        public MAX7219(int chipSelect)
        {
            this.chipSelect = chipSelect;
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
            led = await SpiDevice.FromIdAsync(dis[0].Id, settings);

            led.Write(new byte[] { MAX_SCAN_REG_ADDR, 0x07 });
            SetPower(PowerMode.Normal);
        }

        /// <summary>
        /// Set Register Data and Print
        /// </summary>
        /// <param name="row">Range from 0 to 7</param>
        /// <param name="val">Printed data</param>
        public void SetRow(int row, byte val)
        {
            led.Write(new byte[] { MAX_DIG_REG_ADDR[row], val });
        }

        /// <summary>
        /// Set MAX7219 Decode Mode
        /// </summary>
        /// <param name="mode">Mode</param>
        public void SetDecode(DecodeMode mode)
        {
            led.Write(new byte[] { MAX_DECODE_REG_ADDR, (byte)mode });
        }

        /// <summary>
        /// Set Brightness
        /// </summary>
        /// <param name="val">In range 0-16</param>
        public void SetIntensity(int val)
        {
            if (val >= 0 && val <=16)
            {
                led.Write(new byte[] { MAX_INTENSITY_REG_ADDR, (byte)val });
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
            led.Write(new byte[] { MAX_TEST_REG_ADDR, (byte)mode });
        }

        /// <summary>
        /// Set MAX7219 Power
        /// </summary>
        /// <param name="mode">Mode</param>
        public void SetPower(PowerMode mode)
        {
            led.Write(new byte[] { MAX_POWER_REG_ADDR, (byte)mode });
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            SetPower(PowerMode.Shutdown);
            led.Dispose();
        }
    }
}
