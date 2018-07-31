# ZhangGaoxing's BH1750FVI Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#. BH1750FVI is an digital Ambient Light Sensor IC for I2C bus interface.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/BH1750FVI/02_Image/sensor.jpg)

## Connect
* SDA - Pin3
* SCL - Pin5
* VCC - 3.3V
* GND - GND

## Reference
In Chinese : http://wenku.baidu.com/view/5d0bcafd04a1b0717fd5ddee.html http://wenku.baidu.com/view/8a2e8a7e31b765ce05081443.html

In English : https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/BH1750FVI/01_Datasheet

And his partial code : https://github.com/jeremylindsayni/Magellanic.Sensors.BH1750FVI/blob/master/MeasurementMode.cs

## What Contains
In BH1750FVI.cs file

### It contains two constructors
One constructor should be passed AddressSetting and MeasurementMode, two enumeration type. Another constructor is based on the value above, and added the enumeration type of LightTransmittance to set the light transmittance.

### Three methods 
InitializeAsync(), Read() and Dispose(). InitializeAsync() is used to initializing the sensor. Read() returns a double type sensor data. Dispose() is a cleanup method.

## How to Use
* First, you need to create a BH1750FVI object. After that you should call InitializeAsync() to initialize.
```C#
BH1750FVI sensor = new BH1750FVI(AddressSetting.AddPinLow, MeasurementMode.ContinuouslyHighResolutionMode, LightTransmittance.Hundred);
await sensor.InitializeAsync();
```
* Second, Read().
```C#
double data = sensor.Read();
```
* If you want to close the sensor, call Dispose().
```C#
sensor.Dispose();
```
