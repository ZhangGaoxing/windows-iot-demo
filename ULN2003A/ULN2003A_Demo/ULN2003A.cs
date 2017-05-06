using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace ULN2003A_Demo
{
    /// <summary>
    /// 驱动方式
    /// </summary>
    public enum DrivingMethod
    {
        /// <summary>
        /// .
        /// </summary>
        WaveDrive,
        /// <summary>
        /// 整步驱动
        /// </summary>
        FullStep,
        /// <summary>
        /// 半步驱动
        /// </summary>
        HalfStep
    }

    /// <summary>
    /// 旋转方向（轴心向外）
    /// </summary>
    public enum TurnDirection
    {
        /// <summary>
        /// 左
        /// </summary>
        Left,
        /// <summary>
        /// 右
        /// </summary>
        Right
    }

    class ULN2003A : IDisposable
    {
        private readonly GpioPin[] _gpioPins = new GpioPin[4];

        private readonly GpioPinValue[][] _waveDriveSequence =
        {
            new[] {GpioPinValue.High, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low},
            new[] {GpioPinValue.Low, GpioPinValue.High, GpioPinValue.Low, GpioPinValue.Low},
            new[] {GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.High, GpioPinValue.Low},
            new[] {GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.High}
        };

        // 整步
        private readonly GpioPinValue[][] _fullStepSequence =
        {
            new[] {GpioPinValue.High, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.High},
            new[] {GpioPinValue.High, GpioPinValue.High, GpioPinValue.Low, GpioPinValue.Low},
            new[] {GpioPinValue.Low, GpioPinValue.High, GpioPinValue.High, GpioPinValue.Low},
            new[] {GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.High, GpioPinValue.High }

        };

        // 半步
        private readonly GpioPinValue[][] _halfStepSequence =
        {
            new[] {GpioPinValue.High, GpioPinValue.High, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.High},
            new[] {GpioPinValue.Low, GpioPinValue.High, GpioPinValue.High, GpioPinValue.High, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low},
            new[] {GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.High, GpioPinValue.High, GpioPinValue.High, GpioPinValue.Low, GpioPinValue.Low},
            new[] {GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.Low, GpioPinValue.High, GpioPinValue.High, GpioPinValue.High }
        };

        /// <summary>
        /// 创建 ULN2003A 的新实例
        /// </summary>
        /// <param name="In1">In1 连接的 GPIO 端口</param>
        /// <param name="In2">In2 连接的 GPIO 端口</param>
        /// <param name="In3">In3 连接的 GPIO 端口</param>
        /// <param name="In4">In4 连接的 GPIO 端口</param>
        public ULN2003A(int In1, int In2, int In3, int In4)
        {
            var gpio = GpioController.GetDefault();

            _gpioPins[0] = gpio.OpenPin(In1);
            _gpioPins[1] = gpio.OpenPin(In2);
            _gpioPins[2] = gpio.OpenPin(In3);
            _gpioPins[3] = gpio.OpenPin(In4);

            foreach (var gpioPin in _gpioPins)
            {
                gpioPin.Write(GpioPinValue.Low);
                gpioPin.SetDriveMode(GpioPinDriveMode.Output);
            }
        }

        /// <summary>
        /// 使步进电机旋转给定的角度
        /// </summary>
        /// <param name="degree">旋转角度</param>
        /// <param name="stepAngle">步距角度（步进角 * 减速比）</param>
        /// <param name="direction">旋转方向</param>
        /// <param name="drivingMethod">驱动方式</param>
        /// <returns></returns>
        public async Task TurnAsync(int degree, double stepAngle, TurnDirection direction, DrivingMethod drivingMethod = DrivingMethod.FullStep)
        {
            int steps = 0;
            GpioPinValue[][] methodSequence;

            switch (drivingMethod)
            {
                case DrivingMethod.WaveDrive:
                    methodSequence = _waveDriveSequence;
                    steps = (int)Math.Ceiling(degree / stepAngle);
                    break;
                case DrivingMethod.FullStep:
                    methodSequence = _fullStepSequence;
                    steps = (int)Math.Ceiling(degree / stepAngle);
                    break;
                case DrivingMethod.HalfStep:
                    methodSequence = _halfStepSequence;
                    steps = (int)Math.Ceiling(degree / stepAngle / 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(drivingMethod), drivingMethod, null);
            }

            int counter = 0;

            while (counter < steps)
            {
                for (var j = 0; j < methodSequence[0].Length; j++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        _gpioPins[i].Write(methodSequence[direction == TurnDirection.Left ? i : 3 - i][j]);
                    }
                    await Task.Delay(5);
                    
                    // 一次循环为两步
                    counter += 2;
                    if (counter == steps)
                        break;
                }
            }

            Stop();
        }

        /// <summary>
        /// 使步进电机保持旋转
        /// </summary>
        /// <param name="stepAngle">步距角度（步进角 * 减速比）</param>
        /// <param name="direction">旋转方向</param>
        /// <param name="drivingMethod">驱动方式</param>
        /// <returns></returns>
        public async Task TurnAsync(double stepAngle, TurnDirection direction, DrivingMethod drivingMethod = DrivingMethod.FullStep)
        {
            GpioPinValue[][] methodSequence;

            switch (drivingMethod)
            {
                case DrivingMethod.WaveDrive:
                    methodSequence = _waveDriveSequence;
                    break;
                case DrivingMethod.FullStep:
                    methodSequence = _fullStepSequence;
                    break;
                case DrivingMethod.HalfStep:
                    methodSequence = _halfStepSequence;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(drivingMethod), drivingMethod, null);
            }

            while (true)
            {
                for (var j = 0; j < methodSequence[0].Length; j++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        _gpioPins[i].Write(methodSequence[direction == TurnDirection.Left ? i : 3 - i][j]);
                    }
                    await Task.Delay(5);
                }
            }
        }

        /// <summary>
        /// 使步进电机停止旋转
        /// </summary>
        public void Stop()
        {
            foreach (var gpioPin in _gpioPins)
            {
                gpioPin.Write(GpioPinValue.Low);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            foreach (var gpioPin in _gpioPins)
            {
                gpioPin.Write(GpioPinValue.Low);
                gpioPin.Dispose();
            }
        }
    }
}
