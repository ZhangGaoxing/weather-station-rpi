using System;
using System.Device.I2c;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using WeatherStation.Helper;
using WeatherStation.Model;
using Timer = System.Timers.Timer;

namespace WeatherStation.ConsoleApp
{
    class Program
    {
        private const int Interval = 5000;

        static void Main(string[] args)
        {
            Timer timer = new Timer();
            timer.Interval = Interval;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            
            while (true)
            {
                Thread.Sleep(Interval);
            }
        }

        private static async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            using WeatherContext context = new WeatherContext();

            Weather weather = await GetWeatherAsync();

            context.Add(weather);
            context.SaveChanges();
        }

        private static async Task<Weather> GetWeatherAsync()
        {
            I2cConnectionSettings settings = new I2cConnectionSettings(0, Bme280.SecondaryI2cAddress);
            I2cDevice device = I2cDevice.Create(settings);
            using Bme280 bme = new Bme280(device);

            bme.SetPowerMode(Bmx280PowerMode.Normal);
            bme.SetTemperatureSampling(Sampling.UltraLowPower);
            bme.SetPressureSampling(Sampling.UltraLowPower);
            bme.SetHumiditySampling(Sampling.UltraLowPower);

            double t = Math.Round((await bme.ReadTemperatureAsync()).Celsius, 2);
            double h = Math.Round(await bme.ReadHumidityAsync(), 2);
            double p = Math.Round(await bme.ReadPressureAsync(), 2);

            Console.WriteLine($"Temperature:{t} Humidity:{h} Pressure:{p}");

            return new Weather
            {
                DateTime = DateTime.Now,
                Temperature = t,
                Humidity = h,
                Pressure = p,
                ImageBase64 = GetImageBase64()
            };
        }

        private static string GetImageBase64()
        {
            TerminalHelper.Execute("fswebcam --save /home/pi/image.jpg -d /dev/video0 -r 640x480");

            return FileHelper.FileToBase64("/home/pi/image.jpg");
        }
    }
}
