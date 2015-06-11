#include <Console.h>
#include <AnalogSmooth.h>
#include <Mailbox.h>

const float VIN = 4.54;
const int TEMP_PIN = A1;
const int LED_PIN = 13;
const int BLUE_LED_PIN = 3;
Process tempProcess;
AnalogSmooth as = AnalogSmooth(100);
float previousTemp = 0;
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
  unsigned long currentMillis = millis();
  
  Console.print("Temperature: ");
  Console.println(currentTemp);
  
  if (currentTemp != previousTemp) {
    SendTemperatureTelemetry(currentTemp);      
    previousTemp = currentTemp;
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

void SendTemperatureTelemetry(double temp) {
    Console.print("Sending temperature telemetry: ");
    Console.println(temp);
    
    char buffer[7];
    dtostrf(temp, 4, 2, buffer);
    tempProcess.begin("python");
    tempProcess.addParameter("/root/temperature.py");
    tempProcess.addParameter(buffer);
    tempProcess.runAsynchronously();  
    
    FlashLed();
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

