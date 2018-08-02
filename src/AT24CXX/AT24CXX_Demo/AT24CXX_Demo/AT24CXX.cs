using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace AT24CXX_Demo
{
    class AT24CXX : IDisposable
    {
        private I2cDevice sensor;

        private readonly byte AT_ADDR;

        public AT24CXX(byte address)
        {
            AT_ADDR = address;
        }

        public async Task InitializeAsync()
        {
            var settings = new I2cConnectionSettings(AT_ADDR);
            settings.BusSpeed = I2cBusSpeed.FastMode;

            var controller = await I2cController.GetDefaultAsync();
            sensor = controller.GetDevice(settings);
        }

        public async Task Write(byte start, byte[] buffer)
        {
            foreach (var item in buffer)
            {
                int index = Array.IndexOf(buffer, item);
                sensor.Write(new byte[] { (byte)((start + index) >> 8), (byte)((start + index) & 0xFF), item });
                
                await Task.Delay(10);
            }
            
        }

        public byte[] Read(byte start, int length)
        {
            byte[] buffer = new byte[length];
            byte[] temp = new byte[1];

            for (int i = 0; i < length; i++)
            {
                sensor.WriteRead(new byte[] { (byte)((start + i) >> 8), (byte)((start + i) & 0xFF) }, temp);

                buffer[i] = temp[0];
            }

            return buffer;
        }

        public void Dispose()
        {
            sensor.Dispose();
        }
    }
}
