# ZhangGaoxing's RGB LED Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

PWM in this project is software PWM, which based on the NuGet package **Microsoft.IoT.Lightning**. And the RGB LED is common cathode LED.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/LED/02_Image/sensor.gif)

## Connect
* R - Pin17
* G - Pin27
* B - Pin22
* GND - GND

## How to Use
* First, you need to create a RgbLed object. After that you should call InitializeAsync() to initialize.
```C#
// RedPin, GreenPin, BluePin, PWM Frequency
RgbLed led = new RgbLed(17, 27, 22, 1000, RgbType.CommonCathode);
await led.InitializeAsync();
```
* Second
```C#
// Mode 1
led.SetRedPin(0.5);
led.SetGreenPin(0.2);
led.SetBluePin(0.7);

// Mode 2
led.ShowColor(Colors.Cyan);
```
* If you want to close the sensor, call Dispose().
```C#
led.Dispose();
```
