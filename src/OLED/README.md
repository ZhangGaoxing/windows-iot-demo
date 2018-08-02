# ZhangGaoxing's OLED Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/src/OLED/02_Image/sensor.jpg)

## Connect
* SDA - Pin3
* SCL - Pin5
* VCC - 5V
* GND - GND

## Reference
https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/src/OLED/01_Datasheet

## What Contains
In OLED.cs file
```C#
/// <summary>
/// Initialize the OLED
/// </summary>
public async Task InitializeAsync();

/// <summary>
/// Show character on OLED
/// </summary>
/// <param name="x">x-coordinate</param>
/// <param name="y">y-coordinate / 8 !!!</param>
/// <param name="width">Character Width</param>
/// <param name="height">Character Height</param>
/// <param name="charData">Character Data (common-cathode, column-row, and reverse ou
public void ShowChar(int x, int y, byte width, byte height, byte[] charData);

/// <summary>
/// Send command
/// </summary>
/// <param name="command">Command</param>
private void WriteCommand(byte command);

/// <summary>
/// Send the data which you want to show on the OLED
/// </summary>
/// <param name="data">Data</param>
public void WriteData(byte data);

/// <summary>
/// Set start point (cursor)
/// </summary>
/// <param name="x">x-coordinate</param>
/// <param name="y">y-coordinate / 8 !!!</param>
public void SetPoint(int x, int y);

/// <summary>
/// Fill the OLED with data (input 0xFF to fill, 0x00 to clear)
/// </summary>
public void FillScreen(byte data1, byte data2);

/// <summary>
/// Cleanup
/// </summary>
public void Dispose();

/// <summary>
/// Init command
/// </summary>
private void InitCommand();
```

## How to Use
* First, you need to create a OLED object. After that you should call InitializeAsync() to initialize.
```C#
OLED oled = new OLED();
await oled.InitializeAsync();
```
* Secondly
```C#
oled.ShowChar(0, 0, 16, 16, bytes);
```
* If you want to close the sensor, call Dispose().
```C#
oled.Dispose();
```
