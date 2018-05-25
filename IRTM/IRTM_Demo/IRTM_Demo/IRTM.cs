using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

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

        public async Task InitializeAsync()
        {
            string aqs = SerialDevice.GetDeviceSelector("UART0");                   
            var dis = await DeviceInformation.FindAllAsync(aqs);                    
            serialDevice = await SerialDevice.FromIdAsync(dis[0].Id);  

            serialDevice.BaudRate = 9600;                                           
            serialDevice.Parity = SerialParity.None; 
            serialDevice.StopBits = SerialStopBitCount.One;             
            serialDevice.DataBits = 8;
        }

        public async void Send(byte[] code)
        {
            DataWriter dataWriter = new DataWriter();
            dataWriter.WriteBytes(new byte[] { 0xA1, 0xF1 }.Concat(code).ToArray());
            uint bytesWritten = await serialDevice.OutputStream.WriteAsync(dataWriter.DetachBuffer());
        }

        public async Task<byte[]> ReadAsync(int len = 3)
        {
            DataReader dataReader = new DataReader(serialDevice.InputStream);
            await dataReader.LoadAsync(dataReader.UnconsumedBufferLength);
            byte[] buffer = new byte[len];
            try
            {
                dataReader.ReadBytes(buffer);
            }
            catch
            {
                
            }

            return buffer;
        }

        public async void SetAddress(byte address)
        {
            DataWriter dataWriter = new DataWriter();
            dataWriter.WriteBytes(new byte[] { 0xA1, 0xF2, address, 0x00, 0x00 });
            await serialDevice.OutputStream.WriteAsync(dataWriter.DetachBuffer());
        }

        public async void SetBaudRate(IrtmBaudRate rate)
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

        public SerialDevice GetDevice()
        {
            return serialDevice;
        }

        public void Dispose()
        {
            serialDevice.Dispose();
        }
    }
}
