using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace DS3231_Demo
{
    /// <summary>
    /// DS3231 Raw Data
    /// </summary>
    class DS3231Data
    {
        public int Sec { get; set; }
        public int Min { get; set; }
        public int Hour { get; set; }
        public int Day { get; set; }
        public int Date { get; set; }
        public int Month { get; set; }
        public int Century { get; set; }
        public int Year { get; set; }
    }

    class DS3231 : IDisposable
    {
        #region Address
        private const byte RTC_I2C_ADDR = 0x68;
        private const byte RTC_SEC_REG_ADDR = 0x00;
        private const byte RTC_MIN_REG_ADDR = 0x01;
        private const byte RTC_HOUR_REG_ADDR = 0x02;
        private const byte RTC_DAY_REG_ADDR = 0x03;
        private const byte RTC_DATE_REG_ADDR = 0x04;
        private const byte RTC_MONTH_REG_ADDR = 0x05;
        private const byte RTC_YEAR_REG_ADDR = 0x06;
        private const byte RTC_TEMP_MSB_REG_ADDR = 0x11;
        private const byte RTC_TEMP_LSB_REG_ADDR = 0x12;
        #endregion

        I2cDevice sensor;

        /// <summary>
        /// Initialize the sensor
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            var settings = new I2cConnectionSettings(RTC_I2C_ADDR);
            settings.BusSpeed = I2cBusSpeed.FastMode;

            var controller = await I2cController.GetDefaultAsync();
            sensor = controller.GetDevice(settings);
        }

        /// <summary>
        /// Read Time from DS3231
        /// </summary>
        /// <returns>DS3231 Time</returns>
        public DateTime ReadTime()
        {
            byte[] rawData = new byte[7];

            sensor.WriteRead(new byte[] { RTC_SEC_REG_ADDR }, rawData);

            DS3231Data data = new DS3231Data();

            data.Sec = BCD2Int(rawData[0]);
            data.Min = BCD2Int(rawData[1]);
            data.Hour = BCD2Int(rawData[2]);
            data.Day = BCD2Int(rawData[3]);
            data.Date = BCD2Int(rawData[4]);
            data.Month = BCD2Int((byte)(rawData[5] & 0x1F));
            data.Century = rawData[5] >> 7;
            if (data.Century == 1)
                data.Year = 2000 + BCD2Int(rawData[6]);
            else
                data.Year = 1900 + BCD2Int(rawData[6]);

            return new DateTime(data.Year, data.Month, data.Date, data.Hour, data.Min, data.Sec);
        }

        /// <summary>
        /// Set DS3231 Time
        /// </summary>
        /// <param name="time">Time</param>
        public void SetTime(DateTime time)
        {
            byte[] setData = new byte[8];

            setData[0] = RTC_SEC_REG_ADDR;

            setData[1] = Int2BCD(time.Second);
            setData[2] = Int2BCD(time.Minute);
            setData[3] = Int2BCD(time.Hour);
            setData[4] = Int2BCD(((int)time.DayOfWeek + 7) % 7);
            setData[5] = Int2BCD(time.Day);
            if (time.Year >= 2000)
            {
                setData[6] = (byte)(Int2BCD(time.Month) + 0x80);
                setData[7] = Int2BCD(time.Year - 2000);
            }
            else
            {
                setData[6] = Int2BCD(time.Month);
                setData[7] = Int2BCD(time.Year - 1900);
            }

            sensor.Write(setData);
        }

        /// <summary>
        /// Read DS3231 Temperature
        /// </summary>
        /// <returns></returns>
        public double ReadTemperature()
        {
            byte[] data = new byte[2];

            sensor.WriteRead(new byte[] { RTC_TEMP_MSB_REG_ADDR }, data);

            return data[0] + (data[1] >> 6) * 0.25;
        }

        /// <summary>
        /// Get DS3231 Device
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
       
        /// <summary>
        /// BCD To Int
        /// </summary>
        /// <param name="bcd"></param>
        /// <returns></returns>
        private int BCD2Int(byte bcd)
        {
            return ((bcd / 16 * 10) + (bcd % 16));
        }

        /// <summary>
        /// Int To BCD
        /// </summary>
        /// <param name="dec"></param>
        /// <returns></returns>
        private byte Int2BCD(int dec)
        {
            return (byte)((dec / 10 * 16) + (dec % 10));
        }
    }
}
