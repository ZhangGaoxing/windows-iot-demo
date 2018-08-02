# ZhangGaoxing's HMC5883L Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#. The HMC5883L is a surface-mount, multi-chip module designed for low-field magnetic sensing with a digital interface for applications such as lowcost compassing and magnetometry.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/src/HMC5883L/02_Image/sensor.jpg)

## Connect
* SDA - Pin3
* SCL - Pin5
* VCC - 3.3/5V
* GND - GND

## Reference
https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/src/HMC5883L/Reference

## What Contains
In HMC5883L.cs file
```C#
/// <summary>
/// Initialize
/// </summary>
/// <returns></returns>
public async Task InitializeAsync();
/// <summary>
/// Read raw data from HMC5883L
/// </summary>
/// <returns>Raw data</returns>
public HMC5883LData ReadRaw();
/// <summary>
/// Calculate direction angle
/// </summary>
/// <param name="rawData">HMC5883LData</param>
/// <returns>Angle</returns>
public double RawToDirectionAngle(HMC5883LData rawData);
/// <summary>
/// Cleanup
/// </summary>
public void Dispose();
```

## How to Use
* First, you need to create a HMC5883L object. After that you should call InitializeAsync() to initialize.
```C#
HMC5883L sensor = new HMC5883L(MeasurementMode.Continuous);
await sensor.InitializeAsync();
```
* Secondly
```C#
var data = sensor.ReadRaw();
double angle = sensor.RawToDirectionAngle(data);
```
* If you want to close the sensor, call Dispose().
```C#
sensor.Dispose();
```

## Demo Result
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/HMC5883L/02_Image/result.jpg)