# ZhangGaoxing's MAX7219 Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/src/MAX7219_7Segment/02_Image/sensor.jpg)

## Connect
* DIN - MOSI
* CS - CS0
* CLK - SCLK
* VCC - 5V
* GND - GND

## Reference
https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/src/MAX7219_64LED/Reference

## What Contains
In MAX7219.cs file
```C#
/// <summary>
/// Initialize MAX7219
/// </summary>
public async Task InitializeAsync();
/// <summary>
/// Set Register Data and Print
/// </summary>
/// <param name="index">Segment index, range from 0 to 7</param>
/// <param name="val">Printed data</param>
/// <param name="isDecimal">Does it show Decimal Point (Only for DecodeMode is Digit7)</param>
public void SetSegment(int index, byte value, bool isDecimal = false);
/// <summary>
/// Set MAX7219 Decode Mode
/// </summary>
/// <param name="mode">Mode</param>
public void SetDecode(DecodeMode mode);
/// <summary>
/// Set Brightness
/// </summary>
/// <param name="val">In range 0-16</param>
public void SetIntensity(int val);
/// <summary>
/// Test Display
/// </summary>
/// <param name="mode">Mode</param>
public void DisplayTest(DisplayTestMode mode);
/// <summary>
/// Set MAX7219 Power
/// </summary>
/// <param name="mode">Mode</param>
public void SetPower(PowerMode mode);
/// <summary>
/// Clear the Segment
/// </summary>
public void Clear();
/// <summary>
/// Cleanup
/// </summary>
public void Dispose();
```

## How to Use
* First, you need to create a MAX7219 object. After that you should call InitializeAsync() to initialize.
```C#
MAX7219 led = new MAX7219(0, DecodeMode.Digit7);
await led.InitializeAsync();
led.SetIntensity(5);
```
* Secondly
```C#
for (int i = 0; i < 8; i++)
{
    led.SetSegment(i, (byte)i);
}
```
* If you want to close the sensor, call Dispose().
```C#
led.Dispose();
```
