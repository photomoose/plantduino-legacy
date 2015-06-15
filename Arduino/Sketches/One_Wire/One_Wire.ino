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
}

void loop() {
  int i;
  ds.reset_search();
  
  while (ds.search(addr)) {  
  
    if (OneWire::crc8(addr, 7) != addr[7]) {
        sensorsPresent = false;
        Console.println("CRC is not valid!\n");
    }
    
    Console.print("\nFound: ");
    for (i = 0; i < 8; i++) {
      Console.print(addr[i], HEX);
      Console.print(" ");
    }    
  }
  
  Console.println("\nNo more sensors.");
  
  delay(10000);
}
