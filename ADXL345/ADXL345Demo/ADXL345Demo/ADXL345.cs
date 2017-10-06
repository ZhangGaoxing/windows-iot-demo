using System;
using System.Threading.Tasks;
using Windows.Devices.Spi;
using Windows.Devices.Enumeration;

namespace ADXL345Demo
{
    struct Acceleration
    {
        public double X;
        public double Y;
        public double Z;
    };

    enum GravityRange
    {
        Two = 0x00,
        Four = 0x01,
        Eight = 0x02,
        Sixteen = 0x03
    };

    class ADXL345
    {
        private SpiDevice adxl345;
        private int cs;                                     // Set CS port
        private byte gravityRangeByte;                      // Set range
        private int resolution = 1024;                      // 10 bit resolution
        private int range;
        
        private const byte ADDRESS_POWER_CTL = 0x2D;        // Address of the Power Control register              
        private const byte ADDRESS_DATA_FORMAT = 0x31;      // Address of the Data Format register               
        private const byte ADDRESS_X0 = 0x32;               // Address of the X Axis data register                
        private const byte ADDRESS_Y0 = 0x34;               // Address of the Y Axis data register              
        private const byte ADDRESS_Z0 = 0x36;               // Address of the Z Axis data register               
                             
        private const byte ACCEL_SPI_RW_BIT = 0x80;         // Bit used in SPI transactions to indicate read/write  
        private const byte ACCEL_SPI_MB_BIT = 0x40;         // Bit used to indicate multi-byte SPI transactions    

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chipSelect">CS Port</param>
        /// <param name="gravityRange">Gravity Range, 2G, 4G, 8G, 16G</param>
        public ADXL345(int chipSelect, GravityRange gravityRange)
        {
            cs = chipSelect;
            if (gravityRange == GravityRange.Two)
            {
                range = 4;
            }
            else if (gravityRange == GravityRange.Four)
            {
                range = 8;
            }
            else if (gravityRange == GravityRange.Eight)
            {
                range = 16;
            }
            else if (gravityRange == GravityRange.Sixteen)
            {
                range = 32;
            }
            gravityRangeByte = (byte)gravityRange;
        }

        /// <summary>
        /// Initialize ADXL345
        /// </summary>
        public async Task InitializeAsync()
        {
            var settings = new SpiConnectionSettings(cs);
            settings.ClockFrequency = 5000000;
            settings.Mode = SpiMode.Mode3;
            string aqs = SpiDevice.GetDeviceSelector();
            var dis = await DeviceInformation.FindAllAsync(aqs);
            adxl345 = await SpiDevice.FromIdAsync(dis[0].Id, settings);

            byte[] WriteBuf_DataFormat = new byte[] { ADDRESS_DATA_FORMAT, gravityRangeByte };
            byte[] WriteBuf_PowerControl = new byte[] { ADDRESS_POWER_CTL, 0x08 };

            adxl345.Write(WriteBuf_DataFormat);
            adxl345.Write(WriteBuf_PowerControl);
        }

        /// <summary>
        /// Read data from ADXL345
        /// </summary>
        /// <returns>Acceleration contains double type of X, Y, Z</returns>
        public Acceleration ReadAcceleration()
        {
            Acceleration accel;
            int units = resolution / range;

            byte[] ReadBuf = new byte[6 + 1];
            byte[] RegAddrBuf = new byte[1 + 6];

            RegAddrBuf[0] = ADDRESS_X0 | ACCEL_SPI_RW_BIT | ACCEL_SPI_MB_BIT;
            adxl345.TransferFullDuplex(RegAddrBuf, ReadBuf);
            Array.Copy(ReadBuf, 1, ReadBuf, 0, 6);          

            short AccelerationX = BitConverter.ToInt16(ReadBuf, 0);
            short AccelerationY = BitConverter.ToInt16(ReadBuf, 2);
            short AccelerationZ = BitConverter.ToInt16(ReadBuf, 4);

            accel.X = (double)AccelerationX / units;
            accel.Y = (double)AccelerationY / units;
            accel.Z = (double)AccelerationZ / units;

            return accel;
        }
        
        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            adxl345.Dispose();
        }
    }
}
