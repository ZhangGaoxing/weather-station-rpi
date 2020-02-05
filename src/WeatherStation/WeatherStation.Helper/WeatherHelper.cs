using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherStation.Helper
{
    public class WeatherHelper
    {
        /// <summary>
        /// Get current weather name from Xinzhi
        /// </summary>
        /// <param name="key">Xinzhi api key</param>
        /// <param name="location">Location, like 35.66:123.88</param>
        /// <returns>Weather name</returns>
        public static async Task<string> GetXinzhiWeatherAsync(string key, string location)
        {
            try
            {
                using HttpClient client = new HttpClient();

                var response = await client.GetAsync($"https://api.seniverse.com/v3/weather/now.json?key={key}&location={location}&language=zh-Hans&unit=c");
                string json = await response.Content.ReadAsStringAsync();

                return (string)JsonConvert.DeserializeObject<dynamic>(json).results[0].now.text;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Calculate heat index
        /// </summary>
        /// <param name="temp">Temperature in celsius</param>
        /// <param name="rh">Humidity in percent, like 56(56%)</param>
        /// <returns>Temperature in celsius</returns>
        public static double CalHeatIndex(double temp, double rh)
        {
            double tf = temp * 1.8 + 32;
            double tf2 = Math.Pow(tf, 2);
            double rh2 = Math.Pow(rh, 2);

            double steadman = 0.5 * (tf + 61 + ((tf - 68) * 1.2) + (rh * 0.094));

            if (steadman + tf < 160) 
                return (steadman - 32) / 1.8;

            double rothfuszRegression = (-42.379)
                + (2.04901523 * tf)
                + (10.14333127 * rh)
                - (0.22475541 * tf * rh)
                - (6.83783 * 0.001 * tf2)
                - (5.481717 * 0.01 * rh2)
                + (1.22874 * 0.001 * tf2 * rh)
                + (8.5282 * 0.0001 * tf * rh2)
                - (1.99 * 0.000001 * tf2 * rh2);

            if (rh < 13 && tf >= 80 && tf <= 112)
            {
                rothfuszRegression += ((13 - rh) / 4) * Math.Sqrt((17 - Math.Abs(tf - 95)) / 17);
            }
            else if (rh > 85 && tf >= 80 && tf <= 87)
            {
                rothfuszRegression += ((rh - 85) / 10) * ((87 - tf) / 5);
            }

            return (rothfuszRegression - 32) / 1.8;
        }
    }
}
