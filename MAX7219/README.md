# ZhangGaoxing's MAX7219 Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Image
![](https://github.com/ZhangGaoxing/windows-iot-demo/blob/master/MAX7219/Image.jpg)

## Connect
* DIN - MOSI
* CS - CS0
* CLK - SCLK
* VCC - 5V
* GND - GND

## Reference
https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/MAX7219/Reference

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
/// <param name="row">Range from 0 to 7</param>
/// <param name="val">Printed data</param>
public void SetRow(int row, byte val);
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
/// Cleanup
/// </summary>
public void Dispose();
```

## How to Use
* First, you need to create a MAX7219 object. After that you should call InitializeAsync() to initialize.
```C#
MAX7219 led = new MAX7219(0);
await led.InitializeAsync();
```
* Secondly
```C#
led.SetDecode(DecodeMode.NoDecode);

for (int i = 0; i < 8; i++)
{
    led.SetRow(i, charByte[i]);
}
```
* If you want to close the sensor, call Dispose().
```C#
led.Dispose();
```
