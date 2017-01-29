# ZhangGaoxing's DHT11 Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Connect
* OUT - Pin 3
* VCC - 5V
* GND - GND

## Reference
In Chinese : www.aosong.com

## What Contains
In DHT11.cs file
### One Struct
```C#
// DHT11 data struct
struct DHT11Data
{
  // Temperature - ℃
  public short Temperature;
  // Humidity - %
  public short Humidity;
  // Check the data is true or false
  public bool IsTrue;
}
```
### One Constructor
You need to pass sensor pin value to create a new object.
### Three Methods
```C#
// Initialize DHT11
public void Initialize();
// Read data from DHT11
public DHT11Data Read();
// Cleanup
public void Dispose();
```

## How to Use
* First, you need to create a DHT11 object. After that you should call Initialize() to initialize.
```C#
DHT11 sensor = new DHT11(3);
sensor.Initialize();
```
* Second, Read().
```C#
DHT11Data data = sensor.Read();
```
* If you want to close the sensor, call Dispose().
```C#
sensor.Dispose();
```
