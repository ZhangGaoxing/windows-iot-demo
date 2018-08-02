# ZhangGaoxing's AM2320 Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Connect
* SDA - Pin3
* SCL - Pin5
* VCC - 5V
* GND - GND

## Reference
Datasheet ->  https://wenku.baidu.com/view/0b160c5c5fbfc77da369b191.html

## How to Use
* First, you need to create a AM2320 object. After that you should call InitializeAsync() to initialize.
```C#
AM2320 sensor = new AM2320();
await sensor.InitializeAsync();
```
* Second, Read().
```C#
AM2320Data data = sensor.Read();
```
* If you want to close the sensor, call Dispose().
```C#
sensor.Dispose();
```
