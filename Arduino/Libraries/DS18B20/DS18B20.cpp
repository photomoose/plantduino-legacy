#include "DS18B20.h"

DS18B20::DS18B20(uint8_t pin, byte* address)
{
	this->_sensor = new OneWire(pin);
	this->_address = address;
}

float DS18B20::getTemperature()
{
	double x;
	byte i;
	byte data[9];
	byte LowByte, HighByte, SignBit;
	int TReading;
	  
	this->_sensor->reset();
	this->_sensor->select(this->_address);
	this->_sensor->write(0x44, 1);
	  
	delay(1000);  
	  
	this->_sensor->reset();
	this->_sensor->select(this->_address);
	this->_sensor->write(0xBE);  
	  
	for ( i = 0; i < 9; i++) {           // we need 9 bytes
	  data[i] = this->_sensor->read();
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