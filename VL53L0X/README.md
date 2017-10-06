# ZhangGaoxing's VL53L0X Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Connect
* SDA - Pin3
* SCL - Pin5
* VCC - 5V
* GND - GND

## Reference
This project refered to arduion code VL53L0X.ino, because the datasheet is no useful information... So I don't know how to read and write...

Files and datasheet ->  https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/VL53L0X/Reference

## Screenshot
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/VL53L0X/Reference/Img1.png)
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/VL53L0X/Reference/Img2.png)

## How to Use
* First, you need to create a VL53L0X object. After that you should call InitializeAsync() to initialize.
```C#
VL53L0X sensor = new VL53L0X();
await sensor.InitializeAsync();
```
* Second, Read().
```C#
// Data Struct
VL53L0XData data = sensor.Read();
// Only Distance
short dis = sensor.ReadDistance();
```
* If you want to close the sensor, call Dispose().
```C#
sensor.Dispose();
```
