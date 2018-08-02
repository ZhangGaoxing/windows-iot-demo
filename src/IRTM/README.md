# ZhangGaoxing's YS-IRTM Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Sensor Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/src/IRTM/02_Image/sensor.jpg)

## Reference
<https://github.com/ZhangGaoxing/windows-iot-demo/tree/master/src/IRTM/Reference>

## Connect
* RXD - UART0 TX (PIN 8)
* TXD - UART0 RX (PIN 10)
* VCC - 5V
* GND - GND

## Note
There is one Serial UART available on the RPi2/3: **UART0**
* Pin 8 - UART0 TX
* Pin 10 - UART0 RX

You need add the following capability to the **Package.appxmanifest** file to use Serial UART.
```xml
<Capabilities>
    <DeviceCapability Name="serialcommunication">
        <Device Id="any">
            <Function Type="name:serialPort" />
        </Device>
    </DeviceCapability>
</Capabilities>
```

## What Contains
In IRTM.cs file
```C#
/// <summary>
/// Initialize YS-IRTM
/// </summary>
public async Task InitializeAsync();

/// <summary>
/// Send Order
/// </summary>
/// <param name="code">Order</param>
public async Task SendAsync(byte[] code);

/// <summary>
/// Read Order
/// </summary>
public async Task<byte[]> ReadAsync();

/// <summary>
/// Set YS-IRTM Address
/// </summary>
/// <param name="address">Address from 1 to FF</param>
public async Task SetAddressAsync(byte address);

/// <summary>
/// Set YS-IRTM Baud Rate
/// </summary>
/// <param name="rate">Baud Rate</param>
public async Task SetBaudRateAsync(IrtmBaudRate rate);

/// <summary>
/// Return YS-IRTM
/// </summary>
/// <returns>YS-IRTM</returns>
public SerialDevice GetDevice();

/// <summary>
/// Cleanup
/// </summary>
public void Dispose();
```

## How to Use
* First, you need to create a IRTM object. After that you should call InitializeAsync() to initialize.
```C#
IRTM irtm = new IRTM();
await irtm.InitializeAsync();
```
* Second, SendAsync(). 
```C#
irtm.SendAsync(new byte[] { 0x01, 0x02, 0x03 });
```
* If you want to close the sensor, call Dispose().
```C#
irtm.Dispose();
```
