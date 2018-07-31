# ZhangGaoxing's ADXL345 Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#. The ADXL345 is a small, thin, low power, 3-axis accelerometer with high resolution (13-bit) measurement at up to ±16g.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/ADXL345/02_Image/sensor.jpg)

## Connect
ADXL345 connect to SPI

* VCC - 3.3 V
* GND -  GND
* CS - CS0 - Pin24
* SDO - Pin21
* SDA - Pin19
* SCL - Pin23

## Reference
In Chinese : http://wenku.baidu.com/view/87a1cf5c312b3169a451a47e.html

In English : https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/ADXL345/01_Datasheet

ms-iot some code : https://github.com/ms-iot/samples/tree/develop/SPIAccelerometer/CS

## How to Use
* First, you need to create a ADXL345 object. After that you should call InitializeAsync() to initialize. 
```C#
ADXL345 sensor = new ADXL345(0, GravityRange.Four);
await sensor.InitializeAsync();
```
* Second, ReadAcceleration(). 
```C#
var accel = sensor.ReadAcceleration();
```
* If you want to close the sensor, call Dispose().
```C#
sensor.Dispose();
```
