# ZhangGaoxing's HC-SR501 Demo
This is a Windows IoT Core project on the Raspberry Pi 2/3, coded by C#.

# Connect
* VCC - 5V
* GND - GND
* OUT - GPIO 18- Pin 12

# Reference
In Chinese : http://wenku.baidu.com/view/26ef5a9c49649b6648d747b2.html

# What Contains
In HCSR501.cs file
## One constructor
You need to pass OUT pin value to create a HC-SR501 object.
## Three methods
Initialize(), Read() and Dispose(). Initialize() is used to initializing the sensor. Read() returns a bool type data. Dispose() is a cleanup method.

# How to Use
First, you need to create a HCSR501 object. After that you should call Initialize() to initialize.

Second, Read().

If you want to close the sensor, call Dispose().
