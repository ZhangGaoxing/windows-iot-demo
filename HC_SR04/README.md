# ZhangGaoxing's HC-SR04 Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#. The HC­SR04 ultrasonic sensor uses sonar to determine distance to an object like bats or dolphins do.

HC-SR04 requires 5 volts and it's measuring result is influenced by the surface of an object.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/HC_SR04/02_Image/sensor.jpg)

## Connect
* Vcc - 5V
* Gnd - GND
* Trig - GPIO 17 - Pin 11
* Echo - GPIO 27 - Pin 13

## Reference
In Chinese : http://wenku.baidu.com/view/18288272fc4ffe473368abb5.html

In English : https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/HC_SR04/01_Datasheet

## What Contains
In HCSR04.cs file
### One constructor
You need to pass Trig and Echo pin value to create a HC-SR04 object.
### Three methods
Initialize(), ReadAsync() and Dispose(). Initialize() is used to initializing the sensor. ReadAsync() returns a double type sensor data. Dispose() is a cleanup method.

## How to Use
* First, you need to create a HCSR04 object. After that you should call Initialize() to initialize. 
```C#
HCSR04 sensor = new HCSR04(17, 27);
sensor.Initialize();
```
* Second, ReadAsync(). 
```C#
double distance = await sensor.ReadAsync();
```
* If you want to close the sensor, call Dispose().
```C#
sensor.Dispose();
```
