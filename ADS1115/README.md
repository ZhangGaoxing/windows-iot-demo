# ZhangGaoxing's ADS1115 Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Image
![](https://github.com/ZhangGaoxing/windows-iot-demo/blob/master/ADS1115/Image.png)

## Connect
* ADDR - GND
* SCL - SCL
* SDA - SDA
* VCC - 5V
* GND - GND
* A0 - Your Sensor Output Pin

## Reference
https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/ADS1115/Reference

## What Contains
In ADS1115.cs file
```C#
/// <summary>
/// Constructor
/// </summary>
/// <param name="addr">ADS1115 Address</param>
/// <param name="mux">Input Multiplexer</param>
/// <param name="pga">Programmable Gain Amplifier</param>
/// <param name="rate">Data Rate </param>
public ADS1115(AddressSetting addr = AddressSetting.GND, InputMultiplexeConfig mux = InputMultiplexeConfig.AIN0, PgaConfig pga = PgaConfig.FS4096, DataRate rate = DataRate.SPS128);
/// <summary>
/// Initialize ADS1115
/// </summary>
/// <returns></returns>
public async Task InitializeAsync();
/// <summary>
/// Read Raw Data
/// </summary>
/// <returns>Raw Value</returns>
public short ReadRaw();
/// <summary>
/// Convert Raw Data to Voltage
/// </summary>
/// <param name="val">Raw Data</param>
/// <returns>Voltage</returns>
public double RawToVoltage(short val);
/// <summary>
/// Cleanup
/// </summary>
public void Dispose();
```

## How to Use
* First, you need to create a ADS1115 object. After that you should call InitializeAsync() to initialize.
```C#
ADS1115 adc = new ADS1115(AddressSetting.GND, InputMultiplexeConfig.AIN0, PgaConfig.FS4096, DataRate.SPS860);
await adc.InitializeAsync();
```
* Secondly
```C#
short raw = adc.ReadRaw();
double vol = adc.RawToVoltage(raw);
```
* If you want to close the sensor, call Dispose().
```C#
adc.Dispose();
```
