# ZhangGaoxing's BH1750FVI Demo
This is a Windows IoT Core project on the Raspberry Pi 2/3, coded by C#.

# Reference
In Chinese : http://wenku.baidu.com/view/5d0bcafd04a1b0717fd5ddee.html http://wenku.baidu.com/view/8a2e8a7e31b765ce05081443.html

In English : http://wenku.baidu.com/view/71b5975c804d2b160b4ec0a5.html

And his partial code : https://github.com/jeremylindsayni/Magellanic.Sensors.BH1750FVI/blob/master/MeasurementMode.cs

# What Contains
In BH1750FVI.cs file

## It contains two constructors. 
One constructor should be passed AddressSetting and MeasurementMode, two enumeration type. Another constructor is based on the value above, and added the enumeration type of LightTransmittance to set the light transmittance.

## Three methods. 
InitializeAsync(), Read() and Dispose(). InitializeAsync() is used to initializing the sensor. Read() returns a double type sensor data. Dispose() is a cleanup method.

# How to Use
#### Attention, this project not included try{} catch{}
First, you need to create a BH1750FVI object. After that you should call InitializeAsync() to initialize.
Second, Read().
If you want to close the sensor, call Dispose().
