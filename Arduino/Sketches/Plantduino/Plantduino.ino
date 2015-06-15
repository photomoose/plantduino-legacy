#include <Console.h>
#include <AnalogSmooth.h>
#include <Mailbox.h>
#include <OneWire.h>

OneWire ds(2);
byte sensor1[8] = { 0x28, 0xFF, 0x97, 0x75, 0x70, 0x14, 0x04, 0xDE };
byte sensor2[8] = { 0x28, 0xFF, 0x92, 0x59, 0x70, 0x14, 0x04, 0x6E };

const float VIN = 4.54;
const int TEMP_PIN = A1;
const int LED_PIN = 13;
const int BLUE_LED_PIN = 3;

AnalogSmooth as = AnalogSmooth(100);
float previousTemp1 = 0;
float previousTemp2 = 0;
unsigned long previousMillis = 0;
unsigned long interval = 10000;// * 60 * 5;

void setup() {
  pinMode(LED_PIN, OUTPUT);
  pinMode(BLUE_LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);
  digitalWrite(BLUE_LED_PIN, LOW);
  Bridge.begin();
  Console.begin();
  Mailbox.begin();
}

void loop() {
  String message;
  float currentTemp = GetCurrentTemperature();
  float dsTemp1 = GetDSTemp(sensor1);
  float dsTemp2 = GetDSTemp(sensor2);
  unsigned long currentMillis = millis();
  
  Console.println();
  Console.print("Analog Temperature: ");
  Console.println(currentTemp);
  Console.print("Digital Temperature 1: ");
  Console.println(dsTemp1, 1);
  Console.print("Digital Temperature 2: ");
  Console.println(dsTemp2, 1);
  
  if (dsTemp1 != previousTemp1 || (currentMillis - previousMillis) > 600000) {
    SendTemperatureTelemetry(1, dsTemp1);      
    previousTemp1 = dsTemp1;
    FlashLed();
  }
  
  if (dsTemp2 != previousTemp2 || (currentMillis - previousMillis) > 600000) {
    SendTemperatureTelemetry(2, dsTemp2);      
    previousTemp2 = dsTemp2;
    FlashLed();
  }  
  
  if ((currentMillis - previousMillis) > 600000) {
    previousMillis = currentMillis;
  }
  
  while (Mailbox.messageAvailable())
  {
    Mailbox.readMessage(message);
    Console.print("Message: ");
    Console.println(message);
    
    if (message == "coldspellledon") {
      digitalWrite(BLUE_LED_PIN, HIGH);
    } else {
      digitalWrite(BLUE_LED_PIN, LOW);
    }
  }  
  
  delay(500);
}

void SendTemperatureTelemetry(int deviceId, double temp) {
    Console.print("Sending temperature telemetry: ");
    Console.println(temp);
   
    Process tempProcess;
    tempProcess.begin("python");
    tempProcess.addParameter("/root/temperature.py");
    tempProcess.addParameter(String(deviceId));
    tempProcess.addParameter(String(temp));
    tempProcess.run();
}

float GetCurrentTemperature() {
  int sensorVal = as.analogReadSmooth(TEMP_PIN);
  float tempVoltage = (sensorVal / 1024.0) * VIN;
  
  return (tempVoltage - 0.5) * 100;
}

void FlashLed() {
  digitalWrite(LED_PIN, HIGH);
  delay(100);
  digitalWrite(LED_PIN, LOW);
}

double GetDSTemp(byte *addr) {
  double x;
  byte i;
  byte data[9];
  byte LowByte, HighByte, SignBit;
  int TReading;
  
  ds.reset();
  ds.select(addr);
  ds.write(0x44, 1);
  
  delay(1000);  
  
  ds.reset();
  ds.select(addr);
  ds.write(0xBE);  
  
  Console.print("Scratchpad: ");
  for ( i = 0; i < 9; i++) {           // we need 9 bytes
    data[i] = ds.read();
    Console.print(data[i], HEX);
    Console.print(" ");
  }  
 
  
  LowByte = data[0];
  HighByte = data[1];
  TReading = (HighByte << 8) + LowByte;
  SignBit = TReading & 0x8000;  // test most sig bit
  
  if (SignBit) // negative
  {
    TReading = (TReading ^ 0xffff) + 1; // 2's comp
  }
  
  x = TReading >> 4; 
  x += (TReading & 0xf) * 0.0625;
  
  if (SignBit) // If its negative
  {
     x *= -1;
  }
  
  return x;
}

