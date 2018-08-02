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

namespace VL53L0XDemo
{
    public struct VL53L0XData
    {
        /// <summary>
        /// Millimeter Distance
        /// </summary>
        public short Distance;
        public byte Ambient;
        public byte Signal;
        public byte Status;
    }

    public class VL53L0X : IDisposable
    {
        private I2cDevice sensor;

        private const byte VL53L0X_ADDR = 0x29;
        private const byte VL53L0X_IDENTIFICATION_MODEL_ID = 0xC0;
        private const byte VL53L0X_IDENTIFICATION_REVISION_ID = 0xC2;

        /// <summary>
        /// Initialize
        /// </summary>
        public async Task InitializeAsync()
        {
            var settings = new I2cConnectionSettings(VL53L0X_ADDR);
            settings.BusSpeed = I2cBusSpeed.FastMode;

            var controller = await I2cController.GetDefaultAsync();
            sensor = controller.GetDevice(settings);
        }

        public VL53L0XData Read()
        {
            sensor.Write(new byte[] { 0x00, 0x01 });

            byte[] readBuf = new byte[12];
            sensor.WriteRead(new byte[] { 0x14 }, readBuf);

            VL53L0XData data = new VL53L0XData();
            data.Distance = BitConvert(readBuf[11], readBuf[10]);
            data.Ambient = (byte)BitConvert(readBuf[7], readBuf[6]);
            data.Signal = (byte)BitConvert(readBuf[9], readBuf[8]);
            data.Status = (byte)((readBuf[0] & 0x78) >> 3);

            return data;
        }

        /// <summary>
        /// Read Distance
        /// </summary>
        /// <returns>Return millimeter</returns>
        public short ReadDistance()
        {
            sensor.Write(new byte[] { 0x00, 0x01 });

            byte[] readBuf = new byte[12];
            sensor.WriteRead(new byte[] { 0x14 }, readBuf);

            return BitConvert(readBuf[11], readBuf[10]);
        }

        public byte ReadIdentificationID()
        {
            byte[] readBuf = new byte[1];

            sensor.WriteRead(new byte[] { VL53L0X_IDENTIFICATION_MODEL_ID }, readBuf);

            return readBuf[0];
        }

        public byte ReadIdentificationRevisionID()
        {
            byte[] readBuf = new byte[1];

            sensor.WriteRead(new byte[] { VL53L0X_IDENTIFICATION_REVISION_ID }, readBuf);

            return readBuf[0];
        }

        public I2cDevice GetDevice()
        {
            return sensor;
        }

        public void Dispose()
        {
            sensor.Dispose();
        }

        private short BitConvert(byte lsb, byte msb)
        {
            return (short)(((msb & 0xFF) << 8) | (lsb & 0xFF));
        }

        #region Abandoned
        /*
        public byte ReadAmbient()
        {
            sensor.Write(new byte[] { 0x00, 0x01 });

            byte[] readBuf = new byte[12];
            sensor.WriteRead(new byte[] { 0x14 }, readBuf);

            return BitConvert(readBuf[7], readBuf[6]);
        }

        public byte ReadSignal()
        {
            sensor.Write(new byte[] { 0x00, 0x01 });

            byte[] readBuf = new byte[12];
            sensor.WriteRead(new byte[] { 0x14 }, readBuf);

            return BitConvert(readBuf[9], readBuf[8]);
        }

        public byte ReadStatus()
        {
            sensor.Write(new byte[] { 0x00, 0x01 });

            byte[] readBuf = new byte[12];
            sensor.WriteRead(new byte[] { 0x14 }, readBuf);

            return (byte)((readBuf[0] & 0x78) >> 3);
        }
        */
        #endregion
    }
}
