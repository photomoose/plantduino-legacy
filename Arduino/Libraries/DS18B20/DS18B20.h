#ifndef DS18B20_h
#define DS18B20_h

#include <OneWire.h>

class DS18B20
{
	public:
		DS18B20(uint8_t pin, byte* address);
		float getTemperature();
	private:
		uint8_t _pin;
		byte* _address;
		OneWire* _sensor;
};
#endif