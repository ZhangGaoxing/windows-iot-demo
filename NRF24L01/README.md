# ZhangGaoxing's NRF24L01 Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Image
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/NRF24L01/img.png)

## Connect
![](https://raw.githubusercontent.com/ZhangGaoxing/windows-iot-demo/master/NRF24L01/NRF_bb.jpg)

nRF1
* VCC - 3.3V (Best)
* GND - GND
* MOSI - SPI0 MOSI (GPIO 10)
* MISO - SPI0 MISO (GPIO 9)
* SCK - SPI0 SCLK (GPIO 11)
* CSN - SPI0 CS0 (GPIO 8)
* CE - GPIO 23
* IRQ - GPIO 24
* 
nRF2
* VCC - 3.3V (Best)
* GND - GND
* MOSI - SPI1 MOSI (GPIO 20)
* MISO - SPI1 MISO (GPIO 19)
* SCK - SPI1 SCLK (GPIO 21)
* CSN - SPI1 CS0 (GPIO 16)
* CE - GPIO 5
* IRQ - GPIO 6

## What Contains
In TouchSwitch.cs file
```C#
/// <summary>
/// Initialize
/// </summary>
public async Task InitializeAsync();

/// <summary>
/// Send
/// </summary>
/// <param name="data">Data</param>
public async void Send(byte[] data);

/// <summary>
/// Receive
/// </summary>
/// <param name="length">Packet Size</param>
/// <returns>Data</returns>
public byte[] Receive(byte length);
```
......

## How to Use
* First, you need to create a NRF24L01 object. After that you should call InitializeAsync() to initialize.
```C#
// Create and Initialize
// CSN Pin, CE Pin, IRO Pin, SPI Friendly Name, Receive Packet Size
NRF24L01 sender = new NRF24L01(0, 23, 24, "SPI0", 12);
NRF24L01 receiver = new NRF24L01(0, 5, 6, "SPI1", 12);

await sender.InitializeAsync();
await receiver.InitializeAsync();
```
* Secondly
```C#
sender.Send(Encoding.UTF8.GetBytes("ZhangGaoxing"));
receiver.ReceivedData += Receiver_ReceivedData;

private void Receiver_ReceivedData(object sender, ReceivedDataEventArgs e)
{
    var raw = e.Data.Skip(1).ToArray();
    var res = Encoding.UTF8.GetString(raw);

    Debug.Write("Received Raw Data : ");
    foreach (var item in raw)
    {
        Debug.Write($"{item} ");
    }
    Debug.WriteLine("");
    Debug.WriteLine($"Received Data : {res}");
}
```
* If you want to close the sensor, call Dispose().
```C#
sender.Dispose();
receiver.Dispose();
```
