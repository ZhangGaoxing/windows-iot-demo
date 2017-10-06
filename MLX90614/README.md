# ZhangGaoxing's MLX90614 Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Connect
* SDA - Pin3
* SCL - Pin5
* VCC - 5V
* GND - GND

## Reference
Files and datasheet ->  https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/MLX90614/Reference

## Screenshot
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/MLX90614/Reference/Img.png)

## How to Use
* First, you need to create a MLX90614 object. After that you should call InitializeAsync() to initialize.
```C#
MLX90614 sensor = new MLX90614();
await sensor.InitializeAsync();
```
* Second, Read().
```C#
MLX90614Data data = sensor.Read();
```
* If you want to close the sensor, call Dispose().
```C#
sensor.Dispose();
```
