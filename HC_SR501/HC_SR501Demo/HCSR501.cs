using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace HC_SR501Demo
{
    class HCSR501
    {
        private GpioPin sensor;
        private int pinOut;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pin">OUT Pin</param>
        public HCSR501(int pin)
        {
            pinOut = pin;
        }

        /// <summary>
        /// Initialize the sensor
        /// </summary>
        public void Initialize()
        {
            var gpio = GpioController.GetDefault();

            sensor = gpio.OpenPin(pinOut);

            sensor.SetDriveMode(GpioPinDriveMode.Input);
        }

        /// <summary>
        /// Read from the sensor
        /// </summary>
        /// <returns>Is Detected</returns>
        public bool Read()
        {
            if (sensor.Read() == GpioPinValue.High)
            {
                return true;
            }
            else
            {
                return false;
            }
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
