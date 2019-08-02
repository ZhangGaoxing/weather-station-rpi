using System;
using System.Device.I2c;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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
        private const int Interval = 60000;

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
            if (DateTime.Now.Minute % 10 != 0)
                return;

            using WeatherContext context = new WeatherContext();

            Weather weather = await GetWeatherAsync();

            PostWeiboAsync(weather);

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
                WeatherName = await WeatherHelper.GetXinzhiWeatherAsync(ConfigHelper.Get("Xinzhi:Key"), ConfigHelper.Get("Xinzhi:Location")),
                Temperature = t,
                Humidity = h,
                Pressure = p,
                ImageBase64 = GetImageBase64()
            };
        }

        private static string GetImageBase64()
        {
            TerminalHelper.Execute($"fswebcam --save {ConfigHelper.Get("UsbCamera:ImagePath")} -d /dev/video0 -r 640x480");

            return FileHelper.FileToBase64(ConfigHelper.Get("UsbCamera:ImagePath"));
        }

        private async static void PostWeiboAsync(Weather weather)
        {
            string token = ConfigHelper.Get("Weibo:Token");  // weibo token
            string status = $"{weather.DateTime.ToString("yyyy/MM/dd HH:mm")}    {weather.WeatherName}%0a" +
                    $"温度：{Math.Round(weather.Temperature, 1)} ℃    体感温度：{Math.Round(WeatherHelper.CalHeatIndex(weather.Temperature, weather.Humidity), 1)} ℃%0a" +
                    $"相对湿度：{Math.Round(weather.Humidity)} %25%0a" +
                    $"气压：{Math.Round(weather.Pressure / 100, 2)} hPa%0a" +
                    $"{ConfigHelper.Get("Weibo:StatusUrl")}";

            using HttpClient client = new HttpClient();
            using FileStream imageStream = File.OpenRead(ConfigHelper.Get("UsbCamera:ImagePath"));

            MultipartFormDataContent content = new MultipartFormDataContent
                {
                    { new StringContent(token, Encoding.UTF8), "access_token" },
                    { new StringContent(status, Encoding.UTF8), "status" },
                    { new StreamContent(imageStream, (int)imageStream.Length), "pic", "image.jpg" }
                };

            HttpResponseMessage response = await client.PostAsync("https://api.weibo.com/2/statuses/share.json", content);
        }
    }
}
