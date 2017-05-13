using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace HMC5883L_Demo
{
    class HMC5883LData
    {
        public short X_Axis { get; set; }
        public short Y_Axis { get; set; }
        public short Z_Axis { get; set; }
    }

    /// <summary>
    /// The mode of measuring
    /// </summary>
    enum MeasurementMode
    {
        Continuous = 0x00,
        Single = 0x01
    }

    class HMC5883L : IDisposable
    {
        #region Address
        private const byte HMC_I2C_ADDR = 0x1E;
        private const byte HMC_CONFIG_REG_A_ADDR = 0x01;
        private const byte HMC_MODE_REG_ADDR = 0x02;
        private const byte HMC_X_MSB_REG_ADDR = 0x03;
        private const byte HMC_Z_MSB_REG_ADDR = 0x05;
        private const byte HMC_Y_MSB_REG_ADDR = 0x07;
        #endregion

        I2cDevice sensor;

        private byte measurement;

        public HMC5883L(MeasurementMode measurement)
        {
            this.measurement = (byte)measurement;
        }

        public async Task InitializeAsync()
        {
            var settings = new I2cConnectionSettings(HMC_I2C_ADDR);
            settings.BusSpeed = I2cBusSpeed.FastMode;

            var controller = await I2cController.GetDefaultAsync();
            sensor = controller.GetDevice(settings);

            // In datasheet page 11
            sensor.Write(new byte[] { HMC_CONFIG_REG_A_ADDR, 0x70 });
            // In datasheet page 13
            sensor.Write(new byte[] { HMC_MODE_REG_ADDR, measurement });
        }

        /// <summary>
        /// Read raw data from HMC5883L
        /// </summary>
        /// <returns>Raw data</returns>
        public HMC5883LData ReadRaw()
        {
            byte[] xRead = new byte[2];
            byte[] yRead = new byte[2];
            byte[] zRead = new byte[2];

            sensor.WriteRead(new byte[] { HMC_X_MSB_REG_ADDR }, xRead);
            sensor.WriteRead(new byte[] { HMC_Z_MSB_REG_ADDR }, zRead);
            sensor.WriteRead(new byte[] { HMC_Y_MSB_REG_ADDR }, yRead);

            Array.Reverse(xRead);
            Array.Reverse(yRead);
            Array.Reverse(zRead);

            short x = BitConverter.ToInt16(xRead, 0);
            short y = BitConverter.ToInt16(yRead, 0);
            short z = BitConverter.ToInt16(zRead, 0);

            return new HMC5883LData
            {
                X_Axis = x,
                Y_Axis = y,
                Z_Axis = z
            };
        }

        /// <summary>
        /// Calculate direction angle
        /// </summary>
        /// <param name="rawData">HMC5883LData</param>
        /// <returns>Angle</returns>
        public double RawToDirectionAngle(HMC5883LData rawData)
        {
            double angle = Math.Atan2(rawData.Y_Axis, rawData.X_Axis) * (180 / 3.14159265) + 180;

            return angle;
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
