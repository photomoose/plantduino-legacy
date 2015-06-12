#include <Console.h>

#include <OneWire.h>

OneWire ds(2);
bool sensorsPresent;
byte addr[8];

void setup() {
  Bridge.begin();
  Console.begin(); 

  while (!Console){
    ; // wait for Console port to connect.
  }
  
  Console.println("You're connected to the Console!!!!");

  sensorsPresent = ds.search(addr);  
  
  if (OneWire::crc8(addr, 7) != addr[7]) {
      sensorsPresent = false;
      Console.println("CRC is not valid!\n");
  }  
}

void loop() {
  byte i;
  byte present = 0;
  byte data[12];
  int HighByte, LowByte, TReading, SignBit, Whole, Fract, Tc_10000;
  double x;
  
  if (!sensorsPresent) {
    Console.println("Unable to find sensor.");
    return;
  }
  
  Console.print("R=");
  for (i = 0; i < 8; i++) {
    Console.print(addr[i], HEX);
    Console.print(" ");
  }
  
  ds.reset();
  ds.select(addr);
  ds.write(0x44, 1);
  
  delay(1000);
  
  present = ds.reset();
  ds.select(addr);
  ds.write(0xBE);
  
  Console.print("P=");
  Console.print(present, HEX);
  Console.print(" ");
  
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

  Console.print(x, 4);
  Console.print("-->");
  Console.print(TReading);
  Console.print(":");
  Console.print(Tc_10000);
  
  if (SignBit) // If its negative
  {
     Console.print("-");
  }
  Console.print(Whole);
  Console.print(".");
  if (Fract < 10)
  {  
     Console.print("0");
  }
  Console.print(Fract);

  Console.print("\n");

}
