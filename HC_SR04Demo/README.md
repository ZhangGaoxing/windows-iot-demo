# ZhangGaoxing's HC-SR04 Demo
This is a Windows IoT Core project on the Raspberry Pi 2/3, coded by C#.

# Connect
* Vcc - 5V
* Gnd - GND
* Trig - GPIO 17 - Pin 11
* Echo - GPIO 27 - Pin 13

# Reference
In Chinese : http://wenku.baidu.com/view/18288272fc4ffe473368abb5.html

# Attention
HC-SR04 requires 5 volts and it's measuring result is influenced by the surface of an object.

This project not included try{} catch{}.

# What Contains
In HCSR04.cs file
## One constructor
You need to pass Trig and Echo pin value to create a HC-SR04 object.
## Three methods
Initialize(), ReadAsync() and Dispose(). Initialize() is used to initializing the sensor. ReadAsync() returns a double type sensor data. Dispose() is a cleanup method.

# How to Use
First, you need to create a HCSR04 object. After that you should call Initialize() to initialize. 

Second, ReadAsync(). 

If you want to close the sensor, call Dispose().
