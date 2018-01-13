using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace KT0803L_Demo
{
    /// <summary>
    /// PGA ( Programmable Gain Amplifier ) Gain
    /// </summary>
    enum PGA
    {
        PGA_0dB = 4,
        PGA_4dB = 5,
        PGA_8dB = 6,
        PGA_12dB = 7,
        PGA_N0dB = 0,
        PGA_N4dB = 1,
        PGA_N8dB = 2,
        PGA_N12dB = 3,
    }

    /// <summary>
    /// Transmission Power
    /// </summary>
    enum RFGAIN
    {
        RFGAIN_95_5dBuV = 0,
        RFGAIN_96_5dBuV = 1,
        RFGAIN_97_5dBuV = 2,
        RFGAIN_98_2dBuV = 3,
        RFGAIN_98_9dBuV = 4,
        RFGAIN_100dBuV = 5,
        RFGAIN_101_5dBuV = 6,
        RFGAIN_102_8dBuV = 7,
        RFGAIN_105_1dBuV = 8,
        RFGAIN_105_6dBuV = 9,
        RFGAIN_106_2dBuV = 10,
        RFGAIN_106_5dBuV = 11,
        RFGAIN_107dBuV = 12,
        RFGAIN_107_4dBuV = 13,
        RFGAIN_107_7dBuV = 14,
        RFGAIN_108dBuV = 15,
    }

    /// <summary>
    /// Region
    /// </summary>
    enum Country
    {
        USA,
        JAPAN,
        EUROPE,
        AUSTRALIA,
        CHINA,
    }

    class KT0803L : IDisposable
    {
        private const byte KT_ADDR = 0x3E;

        private I2cDevice sensor;

        /// <summary>
        /// Initialize KT0803L
        /// </summary>
        /// <param name="mhz">FM Channel ( Range from 70Mhz to 108Mhz )</param>
        /// <param name="country">Region</param>
        /// <param name="rfgain">Transmission Power</param>
        /// <param name="pga">PGA ( Programmable Gain Amplifier ) Gain</param>
        public async Task InitializeAsync(double mhz, Country country = Country.CHINA, RFGAIN rfgain = RFGAIN.RFGAIN_98_9dBuV, PGA pga = PGA.PGA_0dB)
        {
            var settings = new I2cConnectionSettings(KT_ADDR);
            settings.BusSpeed = I2cBusSpeed.StandardMode;

            var controller = await I2cController.GetDefaultAsync();
            sensor = controller.GetDevice(settings);

            SetChannel(mhz);
            SetRFGAIN(rfgain);
            SetPGA(pga);
            SetPHTCNST(country);
        }

        /// <summary>
        /// Set FM Channel
        /// </summary>
        /// <param name="mhz">MHz ( Range from 70Mhz to 108Mhz )</param>
        public void SetChannel(double mhz)
        {
            if (mhz < 70 || mhz > 108)
            {
                throw new ArgumentOutOfRangeException("Range from 70Mhz to 108Mhz !");
            }

            byte[] buff = new byte[3];
            sensor.Read(buff);

            int freq, reg0, reg1, reg2;
            reg2 = buff[2];
            reg1 = buff[1];

            freq = (int)(mhz * 20);
            freq &= 0x0FFF;

            if ((freq & 0x01) > 0)
            {
                reg2 |= 0x80;
            }
            else
            {
                reg2 &= ~0x80;
            }

            reg0 = freq >> 1;
            reg1 = (reg1 & 0xF8) | (freq >> 9);

            sensor.Write(new byte[] { 0x02, (byte)reg2 });
            sensor.Write(new byte[] { 0x00, (byte)reg0 });
            sensor.Write(new byte[] { 0x01, (byte)reg1 });
        }

        /// <summary>
        /// Set PGA ( Programmable Gain Amplifier ) Gain
        /// </summary>
        /// <param name="pga">PGA</param>
        public void SetPGA(PGA pga)
        {
            byte[] buff = new byte[1];

            sensor.WriteRead(new byte[] { 0x01 }, buff);
            int reg1 = buff[0];
            sensor.WriteRead(new byte[] { 0x04 }, buff);
            int reg3 = buff[0];

            int pgaVal = (int)pga << 3;
            reg1 = (reg1 & 0xC7) | pgaVal;

            switch (pga)
            {
                case PGA.PGA_0dB:
                    reg3 = (reg3 & 0xCF) | (3 << 4);
                    break;
                case PGA.PGA_4dB:
                    reg3 = (reg3 & 0xCF) | (3 << 4);
                    break;
                case PGA.PGA_8dB:
                    reg3 = (reg3 & 0xCF) | (3 << 4);
                    break;
                case PGA.PGA_12dB:
                    reg3 = (reg3 & 0xCF) | (3 << 4);
                    break;
                case PGA.PGA_N0dB:
                    reg3 = (reg3 & 0xCF) | (0 << 4);
                    break;
                case PGA.PGA_N4dB:
                    reg3 = (reg3 & 0xCF) | (0 << 4);
                    break;
                case PGA.PGA_N8dB:
                    reg3 = (reg3 & 0xCF) | (0 << 4);
                    break;
                case PGA.PGA_N12dB:
                    reg3 = (reg3 & 0xCF) | (0 << 4);
                    break;
                default:
                    break;
            }

            sensor.Write(new byte[] { 0x04, (byte)reg3 });
            sensor.Write(new byte[] { 0x01, (byte)reg1 });
        }

        /// <summary>
        /// Set Transmission Power
        /// </summary>
        /// <param name="pga"></param>
        public void SetRFGAIN(RFGAIN rfgain)
        {
            byte[] buff = new byte[1];

            int reg1, reg2, reg10;
            sensor.WriteRead(new byte[] { 0x01 }, buff);
            reg1 = buff[0];
            sensor.WriteRead(new byte[] { 0x02 }, buff);
            reg2 = buff[0];
            sensor.WriteRead(new byte[] { 0x13 }, buff);
            reg10 = buff[0];

            int rfgainVal = (int)rfgain & 0x0F;

            reg1 = (reg1 & 0x3F) | (rfgainVal << 6);

            if ((rfgainVal & 0x04) > 0)
            {
                reg10 |= 0x80;
            }
            else
            {
                reg10 &= ~0x80;
            }

            if ((rfgainVal & 0x08) > 0)
            {
                reg2 |= 0x40;
            }
            else
            {
                reg2 &= ~0x40;
            }

            if ((int)rfgain >= 8)
            {
                sensor.Write(new byte[] { 0x0E, 0x10 });
            }

            sensor.Write(new byte[] { 0x01, (byte)reg1 });
            sensor.Write(new byte[] { 0x02, (byte)reg2 });
            sensor.Write(new byte[] { 0x13, (byte)reg10 });
        }

        /// <summary>
        /// Set Pre-emphasis Time-Constant
        /// </summary>
        /// <param name="country">Country</param>
        public void SetPHTCNST(Country country)
        {
            byte[] buff = new byte[1];

            int reg2;
            sensor.WriteRead(new byte[] { 0x02 }, buff);
            reg2 = buff[0];

            switch (country)
            {
                case Country.USA:
                    reg2 &= ~0x01;
                    break;
                case Country.JAPAN:
                    reg2 &= ~0x01;
                    break;
                case Country.EUROPE:
                    reg2 |= 0x01;
                    break;
                case Country.AUSTRALIA:
                    reg2 |= 0x01;
                    break;
                case Country.CHINA:
                    reg2 |= 0x01;
                    break;
                default:
                    break;
            }

            sensor.Write(new byte[] { 0x02, (byte)reg2 });
        }

        /// <summary>
        /// Set Mute Mode
        /// </summary>
        /// <param name="isMute">Mute when value is true</param>
        public void SetMute(bool isMute)
        {
            byte[] buff = new byte[1];

            int reg2;
            sensor.WriteRead(new byte[] { 0x02 }, buff);
            reg2 = buff[0];

            if (isMute)
            {
                reg2 |= 0x08;
            }
            else
            {
                reg2 &= ~0x08;
            }

            sensor.Write(new byte[] { 0x02, (byte)reg2 });
        }

        /// <summary>
        /// Set Standby Mode
        /// </summary>
        /// <param name="isStandby">Standby when value is true</param>
        public void SetStandby(bool isStandby)
        {
            byte[] buff = new byte[1];

            int reg4;
            sensor.WriteRead(new byte[] { 0x0B }, buff);
            reg4 = buff[0];

            if (isStandby)
            {
                reg4 |= 0x80;
            }
            else
            {
                reg4 &= ~0x80;
            }

            sensor.Write(new byte[] { 0x0B, (byte)reg4 });
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            sensor.Dispose();
        }

        /// <summary>
        /// Set Crystal Oscillator Disable Control
        /// </summary>
        //void SetXTALD()
        //{
        //    byte[] buff = new byte[1];

        //    int reg15;
        //    sensor.WriteRead(new byte[] { 0x1E }, buff);
        //    reg15 = buff[0];

        //    sensor.Write(new byte[] { 0x1E, (byte)(reg15 | 0x40) });
        //}
    }
}
