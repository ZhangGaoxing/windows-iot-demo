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
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace NRF24L01_Demo
{
    /// <summary>
    /// Power Mode
    /// </summary>
    enum PowerMode
    {
        /// <summary>
        /// On
        /// </summary>
        UP,
        /// <summary>
        /// Off
        /// </summary>
        DOWN
    }

    /// <summary>
    /// Working Mode
    /// </summary>
    enum WorkingMode
    {
        /// <summary>
        /// Receive
        /// </summary>
        RX,
        /// <summary>
        /// Send
        /// </summary>
        TX
    }

    /// <summary>
    /// Data Rate
    /// </summary>
    enum OutputPower
    {
        /// <summary>
        /// -18dBm
        /// </summary>
        N18dBm=0x00,
        /// <summary>
        /// -12dBm
        /// </summary>
        N12dBm = 0x01,
        /// <summary>
        /// -6dBm
        /// </summary>
        N6dBm = 0x02,
        /// <summary>
        /// 0dBm
        /// </summary>
        OdBm = 0x03
    }

    /// <summary>
    /// Data Rate
    /// </summary>
    enum DataRate
    {
        /// <summary>
        /// 1Mbps
        /// </summary>
        DR1Mbps=0x00,
        /// <summary>
        /// 2Mbps
        /// </summary>
        DR2Mbps=0x01
    }

    class ReceivedDataEventArgs : EventArgs
    {
        public byte[] Data { get; }
        public ReceivedDataEventArgs(byte[] data)
        {
            Data = data;
        }
    }

    class NRF24L01 : IDisposable
    {
        private SpiDevice sensor;
        private GpioPin ce;
        private GpioPin irq;

        private int CS;
        private int CE;
        private int IRQ;
        private string spi;

        private byte packetSize;

        #region Address and Command
        private const byte CONFIG = 0x00;
        private const byte EN_AA = 0x01;
        private const byte EN_RXADDR = 0x02;
        public const byte SETUP_AW = 0x03;
        public const byte SETUP_RETR = 0x04;
        public const byte RF_CH = 0x05;
        public const byte RF_SETUP = 0x06;
        public const byte STATUS = 0x07;
        public const byte OBSERVE_TX = 0x08;
        public const byte RPD = 0x09;
        private const byte RX_ADDR_P0 = 0x0A;
        private const byte RX_ADDR_P1 = 0x0B;
        private const byte RX_ADDR_P2 = 0x0C;
        private const byte RX_ADDR_P3 = 0x0D;
        private const byte RX_ADDR_P4 = 0x0E;
        private const byte RX_ADDR_P5 = 0x0F;
        private const byte TX_ADDR = 0x10;
        private const byte RX_PW_P0 = 0x11;
        private const byte RX_PW_P1 = 0x12;
        private const byte RX_PW_P2 = 0x13;
        private const byte RX_PW_P3 = 0x14;
        private const byte RX_PW_P4 = 0x15;
        private const byte RX_PW_P5 = 0x16;
        private const byte FIFO_STATUS = 0x17;

        private const byte R_REGISTER = 0x00;
        private const byte W_REGISTER = 0x20;
        private const byte R_RX_PAYLOAD = 0x61;
        private const byte W_TX_PAYLOAD = 0xA0;
        private const byte FLUSH_TX = 0xE1;
        private const byte FLUSH_RX = 0xE2;
        private const byte REUSE_TX_PL = 0xE3;
        private const byte NOP = 0xFF;
        #endregion

        /// <summary>
        /// Create a NRF24L01 object
        /// </summary>
        /// <param name="CSN">CSN Pin</param>
        /// <param name="CE">CE Pin</param>
        /// <param name="IRQ">IRQ Pin</param>
        /// <param name="SPI">SPI Friendly Name,like 'SPI0', 'SPI1'.</param>
        /// <param name="packetSize">Receive Packet Size</param>
        public NRF24L01(int CSN, int CE, int IRQ, string SPI, byte packetSize)
        {
            this.CS = CSN;
            this.spi = SPI;
            this.CE = CE;
            this.IRQ = IRQ;
            this.packetSize = packetSize;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public async Task InitializeAsync()
        {
            var settings = new SpiConnectionSettings(CS);
            settings.ClockFrequency = 2000000;
            settings.Mode = SpiMode.Mode0;

            string aqs = SpiDevice.GetDeviceSelector(spi);
            var dis = await DeviceInformation.FindAllAsync(aqs);
            sensor = await SpiDevice.FromIdAsync(dis[0].Id, settings);

            var gpio = GpioController.GetDefault();
            ce = gpio.OpenPin(CE);
            irq = gpio.OpenPin(IRQ);
            ce.SetDriveMode(GpioPinDriveMode.Output);
            irq.SetDriveMode(GpioPinDriveMode.Input);
            irq.ValueChanged += Irq_ValueChanged;

            await Task.Delay(50);
            SetRxPayload(packetSize);
            SetAutoAck(false);
            ce.Write(GpioPinValue.Low);
            sensor.Write(new byte[] { FLUSH_TX });
            sensor.Write(new byte[] { FLUSH_RX });            
            sensor.Write(new byte[] { W_REGISTER + CONFIG, 0x3B });
            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Set Receive Packet Size (All Pipe)
        /// </summary>
        /// <param name="payload">Size, from 0 to 32</param>
        public void SetRxPayload(byte payload)
        {
            ce.Write(GpioPinValue.Low);

            if (payload > 32 || payload < 0)
            {
                throw new ArgumentOutOfRangeException("payload", "payload from 0 to 32 !");
            }
            sensor.Write(new byte[] { W_REGISTER + RX_PW_P0, payload });
            sensor.Write(new byte[] { W_REGISTER + RX_PW_P1, payload });
            sensor.Write(new byte[] { W_REGISTER + RX_PW_P2, payload });
            sensor.Write(new byte[] { W_REGISTER + RX_PW_P3, payload });
            sensor.Write(new byte[] { W_REGISTER + RX_PW_P4, payload });
            sensor.Write(new byte[] { W_REGISTER + RX_PW_P5, payload });

            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Set Receive Packet Size
        /// </summary>
        /// <param name="pipe">Pipe, form 0 to 5</param>
        /// <param name="payload">Size, from 0 to 32</param>
        public void SetRxPayload(byte pipe, byte payload)
        {
            if (payload > 32 || payload < 0)
            {
                throw new ArgumentOutOfRangeException("payload", "payload from 0 to 32 !");
            }

            if (pipe > 5 || pipe < 0)
            {
                throw new ArgumentOutOfRangeException("pipe", "pipe from 0 to 5 !");
            }

            ce.Write(GpioPinValue.Low);

            sensor.Write(new byte[] { (byte)(W_REGISTER + RX_PW_P0 + pipe), payload });

            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Set Auto Acknowledgment (All Pipe)
        /// </summary>
        /// <param name="isAutoAck">Is Enable</param>
        public void SetAutoAck(bool isAutoAck)
        {
            ce.Write(GpioPinValue.Low);

            if (isAutoAck)
            {
                sensor.Write(new byte[] { W_REGISTER + EN_AA, 0x3F });
            }
            else
            {
                sensor.Write(new byte[] { W_REGISTER + EN_AA, 0x00 });
            }

            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Set Auto Acknowledgment
        /// </summary>
        /// <param name="pipe">Pipe, form 0 to 5</param>
        /// <param name="isAutoAck">Is Enable</param>
        public void SetAutoAck(byte pipe, bool isAutoAck)
        {
            ce.Write(GpioPinValue.Low);

            byte[] value = new byte[1];
            sensor.TransferSequential(new byte[] { R_REGISTER + EN_AA }, value);

            byte setting;
            if (isAutoAck)
            {
                setting = (byte)(value[0] | (1 << pipe));
            }
            else
            {
                setting = (byte)(value[0] & ~(1 << pipe));
            }

            sensor.Write(new byte[] { W_REGISTER + EN_AA, setting });

            ce.Write(GpioPinValue.High);
        } 

        /// <summary>
        /// Set Receive Pipe (All Pipe)
        /// </summary>
        /// <param name="isEnable"></param>
        public void SetRxPipe(bool isEnable)
        {
            ce.Write(GpioPinValue.Low);

            if (isEnable)
            {
                sensor.Write(new byte[] { W_REGISTER + EN_RXADDR, 0x3F });
            }
            else
            {
                sensor.Write(new byte[] { W_REGISTER + EN_RXADDR, 0x00 });
            }

            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Set Receive Pipe
        /// </summary>
        /// <param name="pipe">Pipe, form 0 to 5</param>
        /// <param name="isEnable">Is Enable</param>
        public void SetRxPipe(byte pipe, bool isEnable)
        {
            ce.Write(GpioPinValue.Low);

            byte[] value = new byte[1];
            sensor.TransferSequential(new byte[] { R_REGISTER + EN_RXADDR }, value);

            byte setting;
            if (isEnable)
            {
                setting = (byte)(value[0] | (1 << pipe));
            }
            else
            {
                setting = (byte)(value[0] & ~(1 << pipe));
            }

            sensor.Write(new byte[] { W_REGISTER + EN_RXADDR, setting });

            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Set Power Mode
        /// </summary>
        /// <param name="mode">Power Mode</param>
        public void SetPowerMode(PowerMode mode)
        {
            ce.Write(GpioPinValue.Low);

            byte[] value = new byte[1];
            sensor.TransferSequential(new byte[] { R_REGISTER + CONFIG }, value);

            byte setting;
            switch (mode)
            {
                case PowerMode.UP:
                    setting = (byte)(value[0] | (1 << 1));
                    break;
                case PowerMode.DOWN:
                    setting = (byte)(value[0] & ~(1 << 1));
                    break;
                default:
                    setting = (byte)(value[0] | (1 << 1));
                    break;
            }

            sensor.Write(new byte[] { W_REGISTER + CONFIG, setting });

            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Set Working Mode
        /// </summary>
        /// <param name="mode">Working Mode</param>
        public void SetWorkingMode(WorkingMode mode)
        {
            ce.Write(GpioPinValue.Low);

            byte[] value = new byte[1];
            sensor.TransferSequential(new byte[] { R_REGISTER + CONFIG }, value);

            byte setting;
            switch (mode)
            {
                case WorkingMode.RX:
                    setting = (byte)(value[0] | 1);
                    sensor.Write(new byte[] { W_REGISTER + CONFIG, setting });
                    ce.Write(GpioPinValue.High);
                    break;
                case WorkingMode.TX:
                    setting = (byte)(value[0] & ~1);
                    sensor.Write(new byte[] { W_REGISTER + CONFIG, setting });
                    break;
                default:
                    setting = (byte)(value[0] | 1);
                    sensor.Write(new byte[] { W_REGISTER + CONFIG, setting });
                    ce.Write(GpioPinValue.High);
                    break;
            }
        }

        /// <summary>
        /// Set Output Power
        /// </summary>
        /// <param name="power">Output Power</param>
        public void SetOutputPower(OutputPower power)
        {
            ce.Write(GpioPinValue.Low);

            byte[] value = new byte[1];
            sensor.TransferSequential(new byte[] { R_REGISTER + RF_SETUP }, value);

            byte setting = (byte)(value[0] & (~0x06) | ((byte)power << 1));

            sensor.Write(new byte[] { W_REGISTER + RF_SETUP, setting });

            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Set Send Rate
        /// </summary>
        /// <param name="rate">Send Rate</param>
        public void SetDataRate(DataRate rate)
        {
            ce.Write(GpioPinValue.Low);

            byte[] value = new byte[1];
            sensor.TransferSequential(new byte[] { R_REGISTER + RF_SETUP }, value);

            byte setting = (byte)(value[0] & (~0x08) | ((byte)rate << 1));

            sensor.Write(new byte[] { W_REGISTER + RF_SETUP, setting });

            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Set Receive Address
        /// </summary>
        /// <param name="pipe">Pipe, form 0 to 5</param>
        /// <param name="address">Address, if (pipe > 1) then (address.Length = 1), else if (pipe = 1 || pipe = 0) then (address.Length = 5)</param>
        public void SetRxAddress(byte pipe, byte[] address)
        {
            if (address.Length > 5)
            {
                throw new ArgumentOutOfRangeException("Array Length must less than 6 !");
            }

            if (pipe > 1 && address.Length > 1)
            {
                throw new ArgumentOutOfRangeException("Array Length must equal 1 when pipe more than 1. Address equal pipe1's address the first 4 byte + one byte your custom !");
            }

            ce.Write(GpioPinValue.Low);

            byte[] buffer = new byte[1 + address.Length];
            buffer[0] = (byte)(W_REGISTER + RX_ADDR_P0 + pipe);
            for (int i = 0; i < address.Length; i++)
            {
                buffer[1 + i] = address[i];
            }

            sensor.Write(buffer);

            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Set Send Address
        /// </summary>
        /// <param name="address">Address, address.Length = 5</param>
        public void SetTxAddress(byte[] address)
        {
            if (address.Length > 5)
            {
                throw new ArgumentOutOfRangeException("Array Length must less than 5 !");
            }

            ce.Write(GpioPinValue.Low);

            byte[] buffer = new byte[1 + address.Length];
            buffer[0] = (byte)(W_REGISTER + TX_ADDR);
            for (int i = 0; i < address.Length; i++)
            {
                buffer[1 + i] = address[i];
            }

            sensor.Write(buffer);

            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Set Working Channel
        /// </summary>
        /// <param name="channel">From 0 to 127</param>
        public void SetChannel(byte channel)
        {
            ce.Write(GpioPinValue.Low);

            sensor.Write(new byte[] { W_REGISTER + RF_CH, channel });

            ce.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="data">Data</param>
        public async void Send(byte[] data)
        {
            SetWorkingMode(WorkingMode.TX);
            await Task.Delay(4);

            byte[] buffer = new byte[1 + data.Length];
            buffer[0] = W_TX_PAYLOAD;
            for (int i = 0; i < data.Length; i++)
            {
                buffer[1 + i] = data[i];
            }
            sensor.Write(buffer);

            ce.Write(GpioPinValue.High);
            await Task.Delay(1);
            ce.Write(GpioPinValue.Low);
            await Task.Delay(10);

            SetWorkingMode(WorkingMode.RX);
            await Task.Delay(1);
        }

        /// <summary>
        /// Receive
        /// </summary>
        /// <param name="length">Packet Size</param>
        /// <returns>Data</returns>
        public byte[] Receive(byte length)
        {
            ce.Write(GpioPinValue.Low);

            byte[] read = new byte[length + 1];
            byte[] write = new byte[length + 1];
            write[0] = R_RX_PAYLOAD;
            sensor.TransferFullDuplex(write, read);

            ce.Write(GpioPinValue.Low);
            sensor.Write(new byte[] { W_REGISTER + STATUS, 0x4E });
            ce.Write(GpioPinValue.High);
            return read;
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            sensor.Dispose();
        }

        /// <summary>
        /// Get NRF24L01
        /// </summary>
        /// <returns>NRF24L01</returns>
        public SpiDevice GetDevice()
        {
            return sensor;
        }

        public delegate void ReceivedDataHandle(object sender, ReceivedDataEventArgs e);

        /// <summary>
        /// Triggering when data was received
        /// </summary>
        public event ReceivedDataHandle ReceivedData;

        private void Irq_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)
            {
                ReceivedData(sender, new ReceivedDataEventArgs(Receive(packetSize)));
            }
        }
    }
}
