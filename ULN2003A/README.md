# ZhangGaoxing's ULN2003A Demo
This is a Windows 10 IoT Core project on the Raspberry Pi 2/3, coded by C#.

## Connect
* In1 - GPIO 26
* In2 - GPIO 13
* In3 - GPIO 6
* In4 - GPIO 5

## Reference
https://github.com/erickbp/IoT/tree/master/Stepper%20Motor

http://edi.wang/post/2016/4/4/windows-10-iot-stepper-motor

## What Have I Changed
* You can input a different __Step Angle__ value to control other stepper motor.
* Added some notes.

## How to Use
* First, you need to create a ULN2003A object
```C#
ULN2003A uln2003a = new ULN2003A(26, 13, 6, 5);
```
* Second, TurnAsync().
```C#
await uln2003a.TurnAsync(degree, stepAngle, TurnDirection.Left, DrivingMethod.FullStep);
```
* If you want to close the sensor, call Dispose().
```C#
uln2003a.Dispose();
```
