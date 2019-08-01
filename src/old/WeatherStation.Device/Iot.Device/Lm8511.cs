using System;
using Iot.Device.Ads1115;
using System.Collections.Generic;
using System.Text;
using System.Device.I2c;
using System.Device.I2c.Drivers;

namespace Iot.Device.Lm8511
{
    public class Lm8511
    {
        private readonly I2cConnectionSettings _adcSetting;

        public double UV { get => GetUv();}

        public Lm8511(I2cConnectionSettings adcSetting)
        {
            _adcSetting = adcSetting;
        }

        private double GetUv()
        {
            UnixI2cDevice pinOut = new UnixI2cDevice(_adcSetting);
            UnixI2cDevice pin3v3 = new UnixI2cDevice(_adcSetting);

            int uvLevel = 0, refLevel = 0;
            using (Ads1115.Ads1115 adcOut=new Ads1115.Ads1115(pinOut, InputMultiplexer.AIN0))
            {
                uvLevel = adcOut.ReadRaw();
            }
            using (Ads1115.Ads1115 adc3v3 = new Ads1115.Ads1115(pinOut, InputMultiplexer.AIN0))
            {
                refLevel = adc3v3.ReadRaw();
            }

            double outputVoltage = 3.3 / refLevel * uvLevel;

            return Map(outputVoltage, 0.99, 2.9, 0.0, 15.0);
        }

        private double Map(double x, double inMin, double inMax, double outMin, double outMax)
        {
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }
}
