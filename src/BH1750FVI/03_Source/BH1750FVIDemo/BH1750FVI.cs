/*
 * MIT License
 * Copyright(c) 2018 - Zhang Yuexin

 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace BH1750FVIDemo
{
    /// <summary>
    /// I2C Address Setting
    /// </summary>
    enum AddressSetting
    {
        /// <summary>
        /// ADD Pin connect to high power level
        /// </summary>
        AddPinHigh = 0x5C,
        /// <summary>
        /// ADD Pin connect to low power level 
        /// </summary>     
        AddPinLow = 0x23            
    };

    /// <summary>
    /// The mode of measuring
    /// </summary>
    enum MeasurementMode
    {
        /// <summary>
        /// Start measurement at 1 lx resolution
        /// </summary>
        ContinuouslyHighResolutionMode = 0x10,
        /// <summary>
        /// Start measurement at 0.5 lx resolution
        /// </summary>
        ContinuouslyHighResolutionMode2 = 0x11,
        /// <summary>
        /// Start measurement at 4 lx resolution
        /// </summary>
        ContinuouslyLowResolutionMode = 0x13,
        /// <summary>
        /// Start measurement at 1 lx resolution once
        /// </summary>
        OneTimeHighResolutionMode = 0x20,
        /// <summary>
        /// Start measurement at 0.5 lx resolution once
        /// </summary>
        OneTimeHighResolutionMode2 = 0x21,
        /// <summary>
        /// Start measurement at 4 lx resolution once
        /// </summary>
        OneTimeLowResolutionMode = 0x23
    }

    /// <summary>
    /// Setting light transmittance
    /// </summary>
    enum LightTransmittance
    {
        Fifty,
        Eighty,
        Hundred,
        Hundred_Twenty,
        Hundred_Fifty,
        Two_Hundred
    }

    class BH1750FVI
    {
        I2cDevice sensor;
        private byte sensorAddress;                             
        private byte sensorMode;
        private byte sensorResolution = 1;
        private double sensorTransmittance = 1;

        private byte registerHighVal = 0x42;
        private byte registerLowVal = 0x65;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="address">Enumeration type of AddressSetting</param>
        /// <param name="mode">Enumeration type of MeasurementMode</param>
        public BH1750FVI(AddressSetting address, MeasurementMode mode)
        {
            sensorAddress = (byte)address;
            sensorMode = (byte)mode;

            if (mode == MeasurementMode.ContinuouslyHighResolutionMode2 || mode == MeasurementMode.OneTimeHighResolutionMode2)
            {
                sensorResolution = 2;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="address">Enumeration type of AddressSetting</param>
        /// <param name="mode">Enumeration type of MeasurementMode</param>
        /// <param name="transmittance">Enumeration type of LightTransmittance</param>
        public BH1750FVI(AddressSetting address, MeasurementMode mode, LightTransmittance transmittance)
        {
            sensorAddress = (byte)address;
            sensorMode = (byte)mode;

            if (mode == MeasurementMode.ContinuouslyHighResolutionMode2 || mode == MeasurementMode.OneTimeHighResolutionMode2)
            {
                sensorResolution = 2;
            }

            switch (transmittance)
            {
                case LightTransmittance.Fifty:
                    {
                        registerHighVal = 0x44;
                        registerLowVal = 0x6A;
                        sensorTransmittance = 0.5;
                    }
                    break;
                case LightTransmittance.Eighty:
                    {
                        registerHighVal = 0x42;
                        registerLowVal = 0x76;
                        sensorTransmittance = 0.8;
                    }
                    break;
                case LightTransmittance.Hundred:
                    {
                        registerHighVal = 0x42;
                        registerLowVal = 0x65;
                    }
                    break;
                case LightTransmittance.Hundred_Twenty:
                    {
                        registerHighVal = 0x41;
                        registerLowVal = 0x7A;
                        sensorTransmittance = 1.2;
                    }
                    break;
                case LightTransmittance.Hundred_Fifty:
                    {
                        registerHighVal = 0x41;
                        registerLowVal = 0x7E;
                        sensorTransmittance = 1.5;
                    }
                    break;
                case LightTransmittance.Two_Hundred:
                    {
                        registerHighVal = 0x41;
                        registerLowVal = 0x73;
                        sensorTransmittance = 2;
                    }
                    break;
            }
        }

        /// <summary>
        /// Initialize BH1750FVI
        /// </summary>
        public async Task InitializeAsync()
        {
            var settings = new I2cConnectionSettings(sensorAddress);
            settings.BusSpeed = I2cBusSpeed.FastMode;                     

            var controller = await I2cController.GetDefaultAsync();
            sensor = controller.GetDevice(settings);

            sensor.Write(new byte[] { 0x01 });
            sensor.Write(new byte[] { registerHighVal });
            sensor.Write(new byte[] { registerLowVal });
        }

        /// <summary>
        /// Read data from BH1750FVI
        /// </summary>
        /// <returns>A double type contains data</returns>
        public double Read()
        {
            byte[] readBuf = new byte[2];

            sensor.WriteRead(new byte[] { sensorMode }, readBuf);

            byte temp = readBuf[0];
            readBuf[0] = readBuf[1];
            readBuf[1] = temp;

            double result = BitConverter.ToUInt16(readBuf, 0) / (1.2 * sensorResolution * sensorTransmittance);

            return result;
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            sensor.Dispose();
        }

        /// <summary>
        /// Get BH1750FVI Device
        /// </summary>
        /// <returns></returns>
        public I2cDevice GetDevice()
        {
            return sensor;
        }
    }
}
