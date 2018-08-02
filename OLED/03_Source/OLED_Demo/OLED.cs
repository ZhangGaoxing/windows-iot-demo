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

namespace OLED_Demo
{
    class OLED : IDisposable
    {
        private I2cDevice sensor;

        const byte OLED_ADDR = 0x3C;

        /// <summary>
        /// Initialize the OLED
        /// </summary>
        public async Task InitializeAsync()
        {
            var settings = new I2cConnectionSettings(OLED_ADDR);
            settings.BusSpeed = I2cBusSpeed.StandardMode;

            var controller = await I2cController.GetDefaultAsync();
            sensor = controller.GetDevice(settings);

            InitCommand();
            FillScreen(0x00, 0x00);
        }

        /// <summary>
        /// Show character on OLED
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate / 8 !!!</param>
        /// <param name="width">Character Width</param>
        /// <param name="height">Character Height</param>
        /// <param name="charData">Character Data (common-cathode, column-row, and reverse output the data)</param>
        public void ShowChar(int x, int y, byte width, byte height, byte[] charData)
        {
            int index = 0;
            for (int i = 0; i < height / 8; i++)
            {
                SetPoint(x, y + i);
                for (int j = 0; j < width; j++)
                {
                    WriteData(charData[index]);
                    index++;
                }
            }
        }

        /// <summary>
        /// Send command
        /// </summary>
        /// <param name="command">Command</param>
        public void WriteCommand(byte command)
        {
            sensor.Write(new byte[] { 0x00, command });
        }

        /// <summary>
        /// Send the data which you want to show on the OLED
        /// </summary>
        /// <param name="data">Data</param>
        public void WriteData(byte data)
        {
            sensor.Write(new byte[] { 0x40, data });
        }

        /// <summary>
        /// Set start point (cursor)
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate / 8 !!!</param>
        public void SetPoint(int x, int y)
        {
            WriteCommand((byte)(0xb0 + y));
            WriteCommand((byte)(((x & 0xf0) >> 4) | 0x10));
            WriteCommand((byte)((x & 0x0f) | 0x01));
        }

        /// <summary>
        /// Fill the OLED with data (input 0xFF to fill, 0x00 to clear)
        /// </summary>
        public void FillScreen(byte data1, byte data2)
        {
            byte x, y;

            WriteCommand(0x02);    /*set lower column address*/
            WriteCommand(0x10);    /*set higher column address*/
            WriteCommand(0xB0);    /*set page address*/
            for (y = 0; y < 8; y++)
            {
                WriteCommand((byte)(0xB0 + y));    /*set page address*/
                WriteCommand(0x02);    /*set lower column address*/
                WriteCommand(0x10);    /*set higher column address*/
                for (x = 0; x < 64; x++)
                {
                    WriteData(data1);
                    WriteData(data2);
                }
            }
        }

        /// <summary>
        /// Get OLED Device
        /// </summary>
        /// <returns>OLED</returns>
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
        /// Init command
        /// </summary>
        private void InitCommand()
        {
            WriteCommand(0xAE);//display off

            WriteCommand(0x00);//set lower column address
            WriteCommand(0x10);//set higher column address

            WriteCommand(0x40);//set display start line

            WriteCommand(0xB0);//set page address

            WriteCommand(0x81);//contrast ratio
            WriteCommand(0xCF);//0~255

            WriteCommand(0xA1);//set segment remap

            WriteCommand(0xA6);//normal / reverse

            WriteCommand(0xA8);//multiplex ratio
            WriteCommand(0x3F);//duty = 1/64

            WriteCommand(0xC8);//Com scan direction

            WriteCommand(0xD3);//set display offset
            WriteCommand(0x00);

            WriteCommand(0xD5);//set osc division
            WriteCommand(0x80);

            WriteCommand(0xD9);//set pre-charge period
            WriteCommand(0xF1);

            WriteCommand(0xDA);//set COM pins
            WriteCommand(0x12);

            WriteCommand(0xDB);//set vcomh
            WriteCommand(0x40);

            WriteCommand(0x8D);//set charge pump enable
            WriteCommand(0x14);

            WriteCommand(0xAF);//display ON
        }
    }
}
