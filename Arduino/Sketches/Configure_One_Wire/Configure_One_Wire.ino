#include <Console.h>
#include <OneWire.h>

OneWire ds(2);
byte addr[8] = { 0x28, 0xFF, 0x97, 0x75, 0x70, 0x14, 0x04, 0xDE };

void setup() {
  Bridge.begin();
  Console.begin(); 

  while (!Console){
    ; // wait for Console port to connect.
  }
  
  Console.println("Connected.");

  //configure();
}

void loop() {
  byte i;
  
  Console.print("R=");
  for (i = 0; i < 8; i++) {
    Console.print(addr[i], HEX);
    Console.print(" ");
  }
  
  ds.reset();
  ds.select(addr);

  readTemp();
}

void configure() {
  Console.print("In configure");
  byte i;
  byte data[9];
  byte sp[3];
 
  
  // Read scratchpad
  ds.reset();
  ds.select(addr); 
  ds.write(0xBE);
  
  Console.print("Scratchpad: ");
  for ( i = 0; i < 9; i++) {           // we need 9 bytes
    data[i] = ds.read();
    Console.print(data[i], HEX);
    Console.print(" ");
  }  
  
  sp[0] = data[2];  // THigh
  sp[1] = data[3];  // TLow
  sp[2] = data[4];  // Config Register
  
  sp[2] &= 0x9F;    // Set to 9-bit resolution (10011111)
  
  
  Console.print("\nWriting scratchpad: ");
  // Write scratchpad
  ds.reset();
  ds.select(addr);  
  ds.write(0x4E);
  for (i = 0; i < 3; i++) {
    ds.write(sp[i]);
    Console.print(sp[i], HEX);
    Console.print(" ");
  }
  
  ds.reset();
  ds.select(addr); 
  ds.write(0xBE);
  
  Console.print("\nWritten Scratchpad: ");
  for ( i = 0; i < 9; i++) {           // we need 9 bytes
    data[i] = ds.read();
    Console.print(data[i], HEX);
    Console.print(" ");
  }  
  
  ds.reset();
  ds.select(addr); 
  ds.write(0x48, 1);
  delay(15);
  Console.println("Copied scratchpad to EEPROM.");
  
  Console.println();
}

void readTemp() {
  byte i;
  byte present = 0;
  byte data[12];
  int HighByte, LowByte, TReading, SignBit, Whole, Fract, Tc_10000;
  double x;
  
  ds.write(0x44, 1);
  
  delay(1000);
  
  present = ds.reset();
  
  if (!present) {
    Console.println("No sensors present.");
  }
  
  ds.select(addr);
  ds.write(0xBE);
  
    for ( i = 0; i < 9; i++) {           // we need 9 bytes
    data[i] = ds.read();
    Console.print(data[i], HEX);
    Console.print(" ");
  }  
  Console.println();
  
  
  LowByte = data[0];
  HighByte = data[1];
  TReading = (HighByte << 8) + LowByte;
  SignBit = TReading & 0x8000;  // test most sig bit
  if (SignBit) // negative
  {
    TReading = (TReading ^ 0xffff) + 1; // 2's comp
  }
  //Tc_10000 = TReading * 625; //(6 * TReading) + TReading / 4;    // multiply by (100 * 0.0625) or 6.25

  x = TReading >> 4;  //Tc_10000 / 10000; //100 // separate off the whole and fractional portions
  x += (TReading & 0xf) * 0.0625;

  if (SignBit) // If its negative
  {
     Console.print("-");
  }
  Console.print(x, 4);
  Console.print("C --> ");
  Console.print(TReading);
 
  Console.print("\n");
  }
