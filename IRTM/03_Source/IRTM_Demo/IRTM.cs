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
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace IRTM_Demo
{
    public enum IrtmBaudRate
    {
        BR4800 = 0x01,
        BR9600 = 0x02,
        BR19200 = 0x03,
        BR57600 = 0x04
    }

    class IRTM : IDisposable
    {
        SerialDevice serialDevice = null;

        /// <summary>
        /// Initialize YS-IRTM
        /// </summary>
        public async Task InitializeAsync()
        {
            string aqs = SerialDevice.GetDeviceSelector("UART0");                   
            var dis = await DeviceInformation.FindAllAsync(aqs);                    
            serialDevice = await SerialDevice.FromIdAsync(dis[0].Id);

            serialDevice.ReadTimeout = TimeSpan.FromSeconds(1);
            serialDevice.BaudRate = 9600;                                           
            serialDevice.Parity = SerialParity.None; 
            serialDevice.StopBits = SerialStopBitCount.One;             
            serialDevice.DataBits = 8;
        }

        /// <summary>
        /// Send Order
        /// </summary>
        /// <param name="code">Order</param>
        public async Task SendAsync(byte[] code)
        {
            DataWriter dataWriter = new DataWriter();
            dataWriter.WriteBytes(new byte[] { 0xA1, 0xF1 }.Concat(code).ToArray());
            uint bytesWritten = await serialDevice.OutputStream.WriteAsync(dataWriter.DetachBuffer());
        }

        /// <summary>
        /// Read Order
        /// </summary>
        public async Task<byte[]> ReadAsync()
        {
            DataReader dataReader = new DataReader(serialDevice.InputStream);
            var count = await dataReader.LoadAsync(dataReader.UnconsumedBufferLength);
            byte[] buffer = null;
            if (count > 0)
            {
                buffer = new byte[count];
                dataReader.ReadBytes(buffer);
            }

            return buffer;
        }

        /// <summary>
        /// Set YS-IRTM Address
        /// </summary>
        /// <param name="address">Address from 1 to FF</param>
        public async Task SetAddressAsync(byte address)
        {
            DataWriter dataWriter = new DataWriter();
            dataWriter.WriteBytes(new byte[] { 0xA1, 0xF2, address, 0x00, 0x00 });
            await serialDevice.OutputStream.WriteAsync(dataWriter.DetachBuffer());
        }

        /// <summary>
        /// Set YS-IRTM Baud Rate
        /// </summary>
        /// <param name="rate">Baud Rate</param>
        public async Task SetBaudRateAsync(IrtmBaudRate rate)
        {
            byte order = 0x02;
            uint br = 9600;
            switch (rate)
            {
                case IrtmBaudRate.BR4800:
                    order = 0x01;
                    br = 4800;
                    break;
                case IrtmBaudRate.BR9600:
                    order = 0x02;
                    br = 9600;
                    break;
                case IrtmBaudRate.BR19200:
                    order = 0x03;
                    br = 19200;
                    break;
                case IrtmBaudRate.BR57600:
                    order = 0x04;
                    br = 57600;
                    break;
                default:
                    break;
            }

            DataWriter dataWriter = new DataWriter();
            dataWriter.WriteBytes(new byte[] { 0xA1, 0xF3, order, 0x00, 0x00 });
            await serialDevice.OutputStream.WriteAsync(dataWriter.DetachBuffer());

            serialDevice.BaudRate = br;
        }

        /// <summary>
        /// Return YS-IRTM
        /// </summary>
        /// <returns>YS-IRTM</returns>
        public SerialDevice GetDevice()
        {
            return serialDevice;
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            serialDevice.Dispose();
        }
    }
}
