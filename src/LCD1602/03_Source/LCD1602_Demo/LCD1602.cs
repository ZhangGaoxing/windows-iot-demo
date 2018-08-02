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
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace LCD1602_Demo
{
    class LCD1602 : IDisposable
    {
        private int RS, RW, E, D0, D1, D2, D3, D4, D5, D6, D7;
        private GpioPin rs, rw, e, d0, d1, d2, d3, d4, d5, d6, d7;
        private GpioPin[] pins;
        private bool isFour = false;

        /// <summary>
        /// Create a LCD1602 object with 8 data line
        /// </summary>
        /// <param name="RS">RS Pin</param>
        /// <param name="RW">RW Pin</param>
        /// <param name="E">E Pin</param>
        /// <param name="D0">D0 Pin</param>
        /// <param name="D1">D1 Pin</param>
        /// <param name="D2">D2 Pin</param>
        /// <param name="D3">D3 Pin</param>
        /// <param name="D4">D4 Pin</param>
        /// <param name="D5">D5 Pin</param>
        /// <param name="D6">D6 Pin</param>
        /// <param name="D7">D7 Pin</param>
        public LCD1602(int RS, int RW, int E, int D0, int D1, int D2, int D3, int D4, int D5, int D6, int D7)
        {
            this.RS = RS;
            this.RW = RW;
            this.E = E;
            this.D0 = D0;
            this.D1 = D1;
            this.D2 = D2;
            this.D3 = D3;
            this.D4 = D4;
            this.D5 = D5;
            this.D6 = D6;
            this.D7 = D7;
        }

        /// <summary>
        /// Create a LCD1602 object with 4 data line
        /// </summary>
        /// <param name="RS">RS Pin</param>
        /// <param name="RW">RW Pin</param>
        /// <param name="E">E Pin</param>
        /// <param name="D4">D4 Pin</param>
        /// <param name="D5">D5 Pin</param>
        /// <param name="D6">D6 Pin</param>
        /// <param name="D7">D7 Pin</param>
        public LCD1602(int RS, int RW, int E, int D4, int D5, int D6, int D7)
        {
            this.RS = RS;
            this.RW = RW;
            this.E = E;
            this.D4 = D4;
            this.D5 = D5;
            this.D6 = D6;
            this.D7 = D7;

            isFour = true;
        }

        /// <summary>
        /// Initialize LCD1602
        /// </summary>
        public async Task InitializeAsync()
        {
            InitPins();
            await InitCommands();
        }

        /// <summary>
        /// Write command
        /// In 4 data line, first write the high 4 bits, then write the low 4 bits. 
        /// </summary>
        /// <param name="command">Command byte</param>
        public async Task WriteCommand(byte command)
        {
            byte value = command;

            rs.Write(GpioPinValue.Low);
            rw.Write(GpioPinValue.Low);
            e.Write(GpioPinValue.Low);

            foreach (var item in pins)
            {
                if ((value & 0x01) == 0)
                {
                    item.Write(GpioPinValue.Low);
                }
                else
                {
                    item.Write(GpioPinValue.High);
                }
                value >>= 1;
            }

            e.Write(GpioPinValue.High);
            await Task.Delay(1);
            e.Write(GpioPinValue.Low);
            await Task.Delay(1);
        }

        /// <summary>
        /// Write data
        /// In 4 data line, first write the high 4 bits, then write the low 4 bits. 
        /// </summary>
        /// <param name="data">Data byte</param>
        public async Task WriteData(byte data)
        {
            byte value = data;

            rs.Write(GpioPinValue.High);
            rw.Write(GpioPinValue.Low);
            e.Write(GpioPinValue.Low);

            foreach (var item in pins)
            {
                if ((value & 0x01) == 0)
                {
                    item.Write(GpioPinValue.Low);
                }
                else
                {
                    item.Write(GpioPinValue.High);
                }
                value >>= 1;
            }

            e.Write(GpioPinValue.High);
            await Task.Delay(1);
            e.Write(GpioPinValue.Low);
            await Task.Delay(1);
        }

        /// <summary>
        /// Print string in LCD
        /// </summary>
        /// <param name="value">string</param>
        public async Task Print(string value)
        {
            var buffer = Encoding.ASCII.GetBytes(value);

            if (!isFour)
            {
                foreach (var item in buffer)
                {
                    await WriteData(item);
                }
            }
            else
            {
                foreach (var item in buffer)
                {
                    await WriteData((byte)(item >> 4));
                    await WriteData((byte)(item & 0x0F));
                }
            }
            
        }

        /// <summary>
        /// Set Print Cursor
        /// </summary>
        /// <param name="x">From 0 to 15</param>
        /// <param name="y">From 0 to 1</param>
        public async Task SetCursor(int x, int y)
        {
            if (!isFour)
            {
                await WriteCommand((byte)(0x80 + x + (y * 0x40)));
            }
            else
            {
                await WriteCommand((byte)((0x80 + x + (y * 0x40)) >> 4));
                await WriteCommand((byte)((0x80 + x + (y * 0x40)) & 0x0F));
            }
        }

        /// <summary>
        /// Clear LCD
        /// </summary>
        public async Task Clear()
        {
            if (!isFour)
            {
                await WriteCommand(0x01);
            }
            else
            {
                await WriteCommand(0x01 >> 4);
                await WriteCommand(0x01 & 0x0F);
            }
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            foreach (var item in pins)
            {
                item.Dispose();
            }
        }

        #region Private Method
        private async Task InitCommands()
        {
            if (!isFour)
            {
                await WriteCommand(0x38);
                await Task.Delay(1);
                await WriteCommand(0x0C);
                await Task.Delay(1);
                await WriteCommand(0x06);
                await Task.Delay(1);
                await WriteCommand(0x01);
                await Task.Delay(1);
            }
            else
            {
                await WriteCommand(0x28 >> 4);
                await WriteCommand(0x28 & 0x0F);
                await Task.Delay(1);
                await WriteCommand(0x0C >> 4);
                await WriteCommand(0x0C & 0x0F);
                await Task.Delay(1);
                await WriteCommand(0x06 >> 4);
                await WriteCommand(0x06 & 0x0F);
                await Task.Delay(1);
                await WriteCommand(0x01 >> 4);
                await WriteCommand(0x01 & 0x0F);
                await Task.Delay(1);
            }
        }

        private void InitPins()
        {
            var gpio = GpioController.GetDefault();

            if (!isFour)
            {
                rs = gpio.OpenPin(RS);
                rw = gpio.OpenPin(RW);
                e = gpio.OpenPin(E);
                d0 = gpio.OpenPin(D0);
                d1 = gpio.OpenPin(D1);
                d2 = gpio.OpenPin(D2);
                d3 = gpio.OpenPin(D3);
                d4 = gpio.OpenPin(D4);
                d5 = gpio.OpenPin(D5);
                d6 = gpio.OpenPin(D6);
                d7 = gpio.OpenPin(D7);

                rs.SetDriveMode(GpioPinDriveMode.Output);
                rw.SetDriveMode(GpioPinDriveMode.Output);
                e.SetDriveMode(GpioPinDriveMode.Output);
                d0.SetDriveMode(GpioPinDriveMode.Output);
                d1.SetDriveMode(GpioPinDriveMode.Output);
                d2.SetDriveMode(GpioPinDriveMode.Output);
                d3.SetDriveMode(GpioPinDriveMode.Output);
                d4.SetDriveMode(GpioPinDriveMode.Output);
                d5.SetDriveMode(GpioPinDriveMode.Output);
                d6.SetDriveMode(GpioPinDriveMode.Output);
                d7.SetDriveMode(GpioPinDriveMode.Output);

                rs.Write(GpioPinValue.Low);
                rw.Write(GpioPinValue.Low);
                e.Write(GpioPinValue.Low);
                d0.Write(GpioPinValue.Low);
                d1.Write(GpioPinValue.Low);
                d2.Write(GpioPinValue.Low);
                d3.Write(GpioPinValue.Low);
                d4.Write(GpioPinValue.Low);
                d5.Write(GpioPinValue.Low);
                d6.Write(GpioPinValue.Low);
                d7.Write(GpioPinValue.Low);

                pins = new GpioPin[8] { d0, d1, d2, d3, d4, d5, d6, d7 };
            }
            else
            {
                rs = gpio.OpenPin(RS);
                rw = gpio.OpenPin(RW);
                e = gpio.OpenPin(E);
                d4 = gpio.OpenPin(D4);
                d5 = gpio.OpenPin(D5);
                d6 = gpio.OpenPin(D6);
                d7 = gpio.OpenPin(D7);

                rs.SetDriveMode(GpioPinDriveMode.Output);
                rw.SetDriveMode(GpioPinDriveMode.Output);
                e.SetDriveMode(GpioPinDriveMode.Output);
                d4.SetDriveMode(GpioPinDriveMode.Output);
                d5.SetDriveMode(GpioPinDriveMode.Output);
                d6.SetDriveMode(GpioPinDriveMode.Output);
                d7.SetDriveMode(GpioPinDriveMode.Output);

                rs.Write(GpioPinValue.Low);
                rw.Write(GpioPinValue.Low);
                e.Write(GpioPinValue.Low);
                d4.Write(GpioPinValue.Low);
                d5.Write(GpioPinValue.Low);
                d6.Write(GpioPinValue.Low);
                d7.Write(GpioPinValue.Low);

                pins = new GpioPin[4] { d4, d5, d6, d7 };
            }

        }
        #endregion
    }
}
