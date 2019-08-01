using System;
using System.Collections.Generic;
using System.Text;
using WeatherStation.Model;
using System.Device.I2c;
using System.Device.I2c.Drivers;
using Iot.Device.Bmx280;
using Iot.Device.Sht3x;
using Iot.Device.Lm8511;

namespace WeatherStation.Device
{
    public class DataAcquisition
    {
        public static Weather GetWeather()
        {
            // Temperature and Humidity Sensor - SHT3x
            I2cConnectionSettings sht3xSettings = new I2cConnectionSettings(1, (byte)Iot.Device.Sht3x.I2cAddress.AddrLow);
            UnixI2cDevice sht3x = new UnixI2cDevice(sht3xSettings);

            // Digital Pressure Sensors - BMP280
            I2cConnectionSettings bmpSettings = new I2cConnectionSettings(1, Bmp280.DefaultI2cAddress);
            UnixI2cDevice bmp280 = new UnixI2cDevice(bmpSettings);

            // Analog to Digital Converter - ADS1115
            I2cConnectionSettings adsSettings = new I2cConnectionSettings(1, (byte)Iot.Device.Ads1115.I2cAddress.GND);

            // Get temperature and humidity
            double temperature = 0, humidity = 0;
            using(Sht3x tempHumiSensor = new Sht3x(sht3x))
            {
                temperature = tempHumiSensor.Temperature.Celsius;
                humidity = tempHumiSensor.Humidity;
            }

            // Get pressure
            double pressure = 0;
            using(Bmp280 pressureSensor=new Bmp280(bmp280))
            {
                pressure = pressureSensor.ReadPressureAsync().Result;
            }

            // Get UV
            Lm8511 uvSensor = new Lm8511(adsSettings);
            double uv = uvSensor.UV;

            Weather weather = new Weather
            {
                DateTime = DateTime.Now,
                Temperature = temperature,
                Humidity = humidity,
                Pressure = pressure,
                UV = uv
            };

            return weather;
        }
    }
}
