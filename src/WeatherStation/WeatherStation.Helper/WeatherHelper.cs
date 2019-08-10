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
            double t = temp * 1.8 + 32;
            double hi;

            hi = -42.379 + 2.04901523 * t + 10.14333127 * rh - .22475541 * t * rh
             - .00683783 * t * t - .05481717 * rh * rh + .00122874 * t * t * rh
             + .00085282 * t * rh * rh - .00000199 * t * t * rh * rh;

            if (hi < 80)
                hi = 0.5 * t + 61.0 + (t - 68.0) * 1.2 + rh * 0.094;
            else
                if (rh < 13 && (t > 80 || t < 112))
                hi -= (13 - rh) / 4.0 * Math.Sqrt((17 - Math.Abs(t - 95)) / 17.0);
            else if (rh > 85 && (t > 80 || t < 87))
                hi += (rh - 85) * (87 - t) / 50.0;

            return (hi - 32) / 1.8;
        }
    }
}
