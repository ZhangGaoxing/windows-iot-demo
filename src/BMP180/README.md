# ZhangGaoxing's BMP180 Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#. The BMP180 is the function compatible successor of the BMP085, a new generation of high precision digital pressure sensors for consumer applications.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/src/BMP180/02_Image/sensor.jpg)

## Connect
* SDA - Pin3
* SCL - Pin5
* VCC - 3.3/5V
* GND - GND

## Reference
In English : https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/src/BMP180/01_Datasheet

## What Contains
In BMP180.cs file
### Two Structs
Calibration and BMP180Data. In Calibration, there are sensor calibration data. BMP180Data is used to return sensor data.
### One Enum Type
Resolution. It is used to set BMP180 sensor pressure sampling accuracy modes.
### One Constructor
You need to pass 'Resolution' to create a new object.
### Seven Methods
Include four private methods and three public methods.
#### Private Methods
```C#
// Read calibration data from sensor
private void ReadCalibrationData();
// Get uncompensated temperature
private async Task<double> ReadUncompensatedTempDataAsync();
// Get uncompensated pressure
private async Task<double> ReadUncompensatedPressDataAsync();
// Get true data by calculating
private void CalculateTrueData(double UT, double UP, out double T, out double P);
```
#### Public Methods
```C#
// Initialize BMP180 sensor
public async Task InitializeAsync();
// Read data from BMP180 sensor
public async Task<BMP180Data> ReadAsync();
// Cleanup
public void Dispose();
```

## How to Use
* First, you need to create a BMP180 object. After that you should call InitializeAsync() to initialize.
```C#
BMP180 sensor = new BMP180(Resolution.UltrHighResolution);
await sensor.InitializeAsync();
```
* Second, ReadAsync().
```C#
var data = await sensor.ReadAsync();
```
* If you want to close the sensor, call Dispose().
```C#
sensor.Dispose();
```
