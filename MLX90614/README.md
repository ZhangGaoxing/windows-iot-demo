# ZhangGaoxing's MLX90614 Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#. The MLX90614 is an Infra Red thermometer for non contact temperature measurements.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/MLX90614/02_Image/sensor.jpg)

## Connect
* SDA - Pin3
* SCL - Pin5
* VCC - 5V
* GND - GND

## Reference
https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/MLX90614/01_Datasheet

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

## Demo Result
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/MLX90614/02_Image/result.png)
