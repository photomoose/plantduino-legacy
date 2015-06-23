#include <Console.h>
#include <Mailbox.h>
#include <DS18B20.h>

byte sensor1[8] = { 0x28, 0xFF, 0x97, 0x75, 0x70, 0x14, 0x04, 0xDE };
byte sensor2[8] = { 0x28, 0xFF, 0x92, 0x59, 0x70, 0x14, 0x04, 0x6E };
DS18B20* ds1;
DS18B20* ds2;

const int MOISTURE_PIN = A0;
const int TEMP_PIN = 2;
const int LED_PIN = 13;
const int BLUE_LED_PIN = 3;
const int MOISTURE_MAX = 1000;

float previousTemp1 = 0;
float previousTemp2 = 0;
unsigned long previousMillis = 0;

void setup() {
  pinMode(LED_PIN, OUTPUT);
  pinMode(BLUE_LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);
  digitalWrite(BLUE_LED_PIN, LOW);
  
  ds1 = new DS18B20(TEMP_PIN, sensor1);
  ds2 = new DS18B20(TEMP_PIN, sensor2);
  
  Bridge.begin();
  Console.begin();
  Mailbox.begin();
}

void loop() {
  String message;

  float dsTemp1 = ds1->getTemperature();
  float dsTemp2 = ds2->getTemperature();
  int moisture = (analogRead(MOISTURE_PIN) / 1000.0) * 100;
  unsigned long currentMillis = millis();
  
  Console.println();
  Console.print("Inside: ");
  Console.println(dsTemp1, 1);
  Console.print("Outside: ");
  Console.println(dsTemp2, 1);
  Console.print("Moisture: ");
  Console.print(moisture);
  Console.println("%");
  Console.print("Timer: ");
  Console.println(currentMillis - previousMillis);
  
//  if (dsTemp1 != previousTemp1 || (currentMillis - previousMillis) > 600000) {
//    SendTemperatureTelemetry("plantduino", "inside", dsTemp1);      
//    previousTemp1 = dsTemp1;
//    FlashLed();
//  }
//  
//  if (dsTemp2 != previousTemp2 || (currentMillis - previousMillis) > 600000) {
//    SendTemperatureTelemetry("plantduino", "outside", dsTemp2);      
//    previousTemp2 = dsTemp2;
//    FlashLed();
//  }  
  
  if ((currentMillis - previousMillis) > 600000) {
    SendTemperatureTelemetry("plantduino", "inside", dsTemp1);
    FlashLed();
    SendTemperatureTelemetry("plantduino", "outside", dsTemp2);
    FlashLed();    
    SendMoistureTelemetry("plantduino", "banana", moisture);
    FlashLed();
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


void SendTemperatureTelemetry(char* deviceId, char* sensorId, double temp) {
  
  Console.print("Sending temperature telemetry for ");
  Console.print(sensorId);
  Console.print(": ");
  Console.println(temp);
   
  Process tempProcess;
  tempProcess.begin("python");
  tempProcess.addParameter("/root/temperature.py");
  tempProcess.addParameter(deviceId);
  tempProcess.addParameter(sensorId);
  tempProcess.addParameter(String(temp));
  tempProcess.run();
}

void SendMoistureTelemetry(char* deviceId, char* sensorId, int moisture) {
  
  Console.print("Sending moisture telemetry for ");
  Console.print(sensorId);
  Console.print(": ");
  Console.println(moisture);
   
  Process tempProcess;
  tempProcess.begin("python");
  tempProcess.addParameter("/root/moisture.py");
  tempProcess.addParameter(deviceId);
  tempProcess.addParameter(sensorId);
  tempProcess.addParameter(String(moisture));
  tempProcess.run();
}

void FlashLed() {
  digitalWrite(LED_PIN, HIGH);
  delay(100);
  digitalWrite(LED_PIN, LOW);
}
