# ZhangGaoxing's KT0803L Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#. KT0803L is designed to process high-fidelity stereo audio signal and transmit modulated FM signal over a short range.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/KT0803L/02_Image/sensor.jpg)

## Connect
* SDA - Pin3
* SCL - Pin5
* VCC - 3.3V
* GND - GND

## Reference
https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/KT0803L/Reference

## What Contains
In KT0803L.cs file
```C#
/// <summary>
/// Initialize KT0803L
/// </summary>
/// <param name="mhz">FM Channel ( Range from 70Mhz to 108Mhz )</param>
/// <param name="country">Region</param>
/// <param name="rfgain">Transmission Power</param>
/// <param name="pga">PGA ( Programmable Gain Amplifier ) Gain</param>
public async Task InitializeAsync(double mhz, Country country = Country.CHINA, RFGAIN rfgain = RFGAIN.RFGAIN_98_9dBuV, PGA pga = PGA.PGA_0dB);

/// <summary>
/// Set FM Channel
/// </summary>
/// <param name="mhz">MHz ( Range from 70Mhz to 108Mhz )</param>
public void SetChannel(double mhz);

/// <summary>
/// Set PGA ( Programmable Gain Amplifier ) Gain
/// </summary>
/// <param name="pga">PGA</param>
public void SetPGA(PGA pga);

/// <summary>
/// Set Transmission Power
/// </summary>
/// <param name="pga"></param>
public void SetRFGAIN(RFGAIN rfgain);

/// <summary>
/// Set Pre-emphasis Time-Constant
/// </summary>
/// <param name="country">Country</param>
public void SetPHTCNST(Country country);

/// <summary>
/// Set Mute Mode
/// </summary>
/// <param name="isMute">Mute when value is true</param>
public void SetMute(bool isMute);

/// <summary>
/// Set Standby Mode
/// </summary>
/// <param name="isStandby">Standby when value is true</param>
public void SetStandby(bool isStandby);

/// <summary>
/// Set Bass Boost
/// </summary>
/// <param name="boost">Boost Mode</param>
public void SetBassBoost(BassBoost boost);

/// <summary>
/// Cleanup
/// </summary>
public void Dispose();
```

## How to Use
* First, you need to create a KT0803L object. After that you should call InitializeAsync() to initialize.
```C#
KT0803L kt = new KT0803L();
// 108 MHz
await kt.InitializeAsync(108);
```
* And you can input the audio through the 3.5mm earphone jack. After that, turn on your radio, listen to FM 108 MHz.
* If you want to close the sensor, call Dispose().
```C#
kt.Dispose();
```
