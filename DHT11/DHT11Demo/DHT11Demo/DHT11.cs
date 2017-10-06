using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace DHT11Demo
{
    /// <summary>
    /// DHT11 data struct
    /// </summary>
    struct DHT11Data
    {
        /// <summary>
        /// Temperature - ℃
        /// </summary>
        public short Temperature;
        /// <summary>
        /// Humidity - %
        /// </summary>
        public short Humidity;
        /// <summary>
        /// Check the data is true or false
        /// </summary>
        public bool IsTrue;
    }

    class DHT11
    {
        GpioPin sensor;
        int pin;
        Stopwatch sw = new Stopwatch();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gpioPin">DHT11 OUT Pin value</param>
        public DHT11(int gpioPin)
        {
            pin = gpioPin;
        }

        /// <summary>
        /// Initialize DHT11
        /// </summary>
        public void Initialize()
        {
            var gpio = GpioController.GetDefault();
            sensor = gpio.OpenPin(pin);
            sensor.SetDriveMode(GpioPinDriveMode.Output);
            sensor.Write(GpioPinValue.High);

            sw.Restart();
            while (sw.Elapsed.TotalSeconds <= 1) ;
            sw.Stop();
        }

        /// <summary>
        /// Read data from DHT11
        /// </summary>
        /// <returns>DHT11 data struct</returns>
        public DHT11Data Read()
        {
            string readString = null;
            string[] readBuf = new string[5];
            byte[] tempBuf = new byte[2];
            byte[] humBuf = new byte[2];
            byte check;
            DHT11Data data;

            sensor.Write(GpioPinValue.Low);

            sw.Restart();
            while (sw.Elapsed.TotalMilliseconds <= 18) ;
            sw.Stop();

            sensor.Write(GpioPinValue.High);

            sw.Restart();
            while (sw.Elapsed.TotalMilliseconds <= 0.03) ;
            sw.Stop();

            sensor.SetDriveMode(GpioPinDriveMode.Input);

            sw.Restart();
            while (sw.Elapsed.TotalMilliseconds <= 0.08) ;
            sw.Stop();

            sw.Restart();
            while (sw.Elapsed.TotalMilliseconds <= 0.08) ;
            sw.Stop();

            for (int i = 0; i < 40; i++)
            {
                string result = "0";

                while (sensor.Read() == GpioPinValue.Low) ;

                if (sensor.Read() == GpioPinValue.High)
                {
                    sw.Restart();
                    while (sw.Elapsed.TotalMilliseconds <= 0.028) ;
                    sw.Stop();

                    if (sensor.Read() == GpioPinValue.High)
                    {
                        result = "1";
                        sw.Restart();
                        while (sw.Elapsed.TotalMilliseconds <= 0.05) ;
                        sw.Stop();
                    }
                }

                readString += result;
                if (i == 7 || i == 15 || i == 23 || i == 31)
                {
                    readString += ",";
                }
            }

            readBuf = readString.Split(',');
            tempBuf[0] = Convert.ToByte(readBuf[2], 2);
            tempBuf[1] = Convert.ToByte(readBuf[3], 2);
            humBuf[0] = Convert.ToByte(readBuf[0], 2);
            humBuf[1] = Convert.ToByte(readBuf[1], 2);
            check = Convert.ToByte(readBuf[4], 2);

            data.Temperature = BitConverter.ToInt16(tempBuf, 0);
            data.Humidity = BitConverter.ToInt16(humBuf, 0);

            if (tempBuf[0] + tempBuf[1] + humBuf[0] + humBuf[1] == check)
            {
                data.IsTrue = true;
            }
            else
            {
                data.IsTrue = false;
            }

            sensor.SetDriveMode(GpioPinDriveMode.Output);
            sensor.Write(GpioPinValue.High);

            return data;
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
