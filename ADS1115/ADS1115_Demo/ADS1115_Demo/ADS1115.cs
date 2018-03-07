using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;

namespace ADS1115_Demo
{
    /// <summary>
    /// I2C Address Setting
    /// </summary>
    enum AddressSetting
    {
        /// <summary>
        /// ADDR Pin connect to GND
        /// </summary>
        GND = 0x48,
        /// <summary>
        /// ADDR Pin connect to VCC
        /// </summary>     
        VCC = 0x49,
        /// <summary>
        /// ADDR Pin connect to SDA
        /// </summary>
        SDA = 0x4A,
        /// <summary>
        /// ADDR Pin connect to SCL
        /// </summary>
        SCL = 0x4B
    };

    /// <summary>
    ///  Configure the Input Multiplexer
    /// </summary>
    enum InputMultiplexeConfig
    {
        /// <summary>
        ///  AINP = AIN0 and AINN = GND 
        /// </summary>
        AIN0 = 0x04,
        /// <summary>
        /// AINP = AIN1 and AINN = GND
        /// </summary>
        AIN1 = 0x05,
        /// <summary>
        /// AINP = AIN2 and AINN = GND
        /// </summary>
        AIN2 = 0x06,
        /// <summary>
        /// AINP = AIN3 and AINN = GND
        /// </summary>
        AIN3 = 0x07,
        /// <summary>
        /// AINP = AIN0 and AINN = AIN1
        /// Measure the Voltage between AINP and AINN
        /// </summary>
        AIN0_AIN1 = 0x00,
        /// <summary>
        /// AINP = AIN0 and AINN = AIN3
        /// Measure the Voltage between AINP and AINN
        /// </summary>
        AIN0_AIN3 = 0x00,
        /// <summary>
        /// AINP = AIN1 and AINN = AIN3
        /// Measure the Voltage between AINP and AINN
        /// </summary>
        AIN1_AIN3 = 0x00,
        /// <summary>
        /// AINP = AIN2 and AINN = AIN3
        /// Measure the Voltage between AINP and AINN
        /// </summary>
        AIN2_AIN3 = 0x00,
    }

    /// <summary>
    ///  Configure the Programmable Gain Amplifier
    /// </summary>
    enum PgaConfig
    {
        /// <summary>
        /// ±6.144V
        /// </summary>
        FS6144 = 0x00,
        /// <summary>
        /// ±4.096V
        /// </summary>
        FS4096 = 0x01,
        /// <summary>
        /// ±2.048V
        /// </summary>
        FS2048 = 0x02,
        /// <summary>
        /// ±1.024V
        /// </summary>
        FS1024 = 0x03,
        /// <summary>
        /// ±0.512V
        /// </summary>
        FS512 = 0x04,
        /// <summary>
        /// ±0.256V
        /// </summary>
        FS256 = 0x05
    }

    /// <summary>
    /// Set the Mode of ADS1115
    /// </summary>
    enum DeviceMode
    {
        Continuous = 0x00,
        PowerDown = 0x01
    }

    /// <summary>
    ///  Control the Data Rate 
    /// </summary>
    enum DataRate
    {
        SPS8 = 0x00,
        SPS16 = 0x01,
        SPS32 = 0x02,
        SPS64 = 0x03,
        SPS128 = 0x04,
        SPS250 = 0x05,
        SPS475 = 0x06,
        SPS860 = 0x07
    }

    /// <summary>
    ///  Comparator Mode of Operation
    /// </summary>
    enum ComparatorMode
    {
        Traditional = 0x00,
        Window = 0x01
    }

    /// <summary>
    /// Controls the Polarity of the ALERT Pin
    /// </summary>
    enum ComparatorPolarity
    {
        Low = 0x00,
        High = 0x01
    }

    enum ComparatorLatching
    {
        NonLatching = 0x00,
        Latching = 0x01
    }

    enum ComparatorQueue
    {
        AssertAfterOne = 0x00,
        AssertAfterTwo = 0x01,
        AssertAfterFour = 0x02,
        Disable = 0x03
    }

    class ADS1115 : IDisposable
    {
        const byte ADC_CONVERSION_REG_ADDR = 0x00;
        const byte ADC_CONFIG_REG_ADDR = 0x01;

        I2cDevice adc = null;

        private byte adcAddr;
        private byte adcMux;
        private byte adcPga;
        private byte adcRate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="addr">ADS1115 Address</param>
        /// <param name="mux">Input Multiplexer</param>
        /// <param name="pga">Programmable Gain Amplifier</param>
        /// <param name="rate">Data Rate </param>
        public ADS1115(AddressSetting addr = AddressSetting.GND, InputMultiplexeConfig mux = InputMultiplexeConfig.AIN0, PgaConfig pga = PgaConfig.FS4096, DataRate rate = DataRate.SPS128)
        {
            adcAddr = (byte)addr;
            adcMux = (byte)mux;
            adcPga = (byte)pga;
            adcRate = (byte)rate;
        }

        /// <summary>
        /// Initialize ADS1115
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            var settings = new I2cConnectionSettings(adcAddr);
            settings.BusSpeed = I2cBusSpeed.FastMode;

            var controller = await I2cController.GetDefaultAsync();
            adc = controller.GetDevice(settings);

            byte configHi = (byte)((adcMux << 4) +
                            (adcPga << 1) +
                            (byte)DeviceMode.Continuous);

            byte configLo = (byte)((adcRate << 5) +
                            ((byte)(ComparatorMode.Traditional) << 4) +
                            ((byte)ComparatorPolarity.Low << 3) +
                            ((byte)ComparatorLatching.NonLatching << 2) +
                            (byte)ComparatorQueue.Disable);

            adc.Write(new byte[] { ADC_CONFIG_REG_ADDR, configHi, configLo });
        }

        /// <summary>
        /// Read Raw Data
        /// </summary>
        /// <returns>Raw Value</returns>
        public short ReadRaw()
        {
            short val;
            var data = new byte[2];

            adc.WriteRead(new byte[] { ADC_CONVERSION_REG_ADDR }, data);

            Array.Reverse(data);
            val = BitConverter.ToInt16(data, 0);

            return val;
        }

        /// <summary>
        /// Convert Raw Data to Voltage
        /// </summary>
        /// <param name="val">Raw Data</param>
        /// <returns>Voltage</returns>
        public double RawToVoltage(short val)
        {
            double voltage;
            double resolution;

            if (adcPga == 0x00)
            {
                voltage = 6.144;
            }
            else if (adcPga == 0x01)
            {
                voltage = 4.096;
            }
            else if (adcPga == 0x02)
            {
                voltage = 2.048;
            }
            else if (adcPga == 0x03)
            {
                voltage = 1.024;
            }
            else if (adcPga == 0x04)
            {
                voltage = 0.512;
            }
            else
            {
                voltage = 0.256;
            }

            if (adcMux <= 0x03)
            {
                resolution = 65535.0;
            }
            else
            {
                resolution = 32768.0;
            }

            return val * (voltage / resolution);
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            adc.Dispose();
        }

        public I2cDevice GetDevice()
        {
            return adc;
        }
    }
}
