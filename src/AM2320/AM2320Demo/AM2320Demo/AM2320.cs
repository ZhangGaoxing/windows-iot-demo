using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace AM2320Demo
{
    public struct AM2320Data
    {
        public double Temperature;
        public double Humidity;
    }

    public class AM2320 : IDisposable
    {
        private I2cDevice sensor;

        private const byte AM2320_ADDR = 0x5C;

        /// <summary>
        /// Initialize
        /// </summary>
        public async Task InitializeAsync()
        {
            var settings = new I2cConnectionSettings(AM2320_ADDR);
            settings.BusSpeed = I2cBusSpeed.StandardMode;

            var controller = await I2cController.GetDefaultAsync();
            sensor = controller.GetDevice(settings);
        }

        public AM2320Data Read()
        {
            byte[] readBuf = new byte[4];

            sensor.WriteRead(new byte[] { 0x03, 0x00, 0x04 }, readBuf);

            double rawH = BitConverter.ToInt16(readBuf, 0);
            double rawT = BitConverter.ToInt16(readBuf, 2);

            AM2320Data data = new AM2320Data
            {
                Temperature = rawT / 10.0,
                Humidity = rawH / 10.0
            };

            return data;
        }

        public I2cDevice GetDevice()
        {
            return sensor;
        }

        public void Dispose()
        {
            sensor.Dispose();
        }
    }
}
