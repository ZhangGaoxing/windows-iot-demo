# ZhangGaoxing's TouchSwitch Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/src/TouchSwitch/02_Image/sensor.jpg)

## Connect
* VCC - 5V
* GND - GND
* I/O - GPIO 4

## What Contains
In TouchSwitch.cs file
```C#
/// <summary>
/// Initialize
/// </summary>
public void Initialize();

/// <summary>
/// Read GpioPinValue
/// </summary>
/// <returns>GpioPinValue</returns>
public GpioPinValue Read();

/// <summary>
/// Triggering when SwitchState changes
/// </summary>
public event SwitchStateChangedHandle SwitchStateChanged;
```

## How to Use
* First, you need to create a TouchSwitch object. After that you should call Initialize() to initialize.
```C#
// Create and Initialize
TouchSwitch touchSwitch = new TouchSwitch(4);
touchSwitch.Initialize();
```
* Secondly
```C#
// Triggering when SwitchState changes
touchSwitch.SwitchStateChanged += TouchSwitch_SwitchStateChanged;

private void TouchSwitch_SwitchStateChanged(object sender, SwitchStateChangedEventArgs e)
{
    // TODO
    Debug.WriteLine($"Current Switch State : {e.SwitchState.ToString()}");
}
```
* If you want to close the sensor, call Dispose().
```C#
touchSwitch.Dispose();
```

## Demo Result
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/src/TouchSwitch/02_Image/result.png)