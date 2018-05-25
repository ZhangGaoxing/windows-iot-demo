using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
