using Microsoft.Extensions.Configuration;

namespace WeatherStation.Helper
{
    /// <summary>
    /// JSON 配置文件帮助类
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// Configuration, the default file path just contains "appsettings.json".
        /// </summary>
        public static IConfigurationRoot Configuration { get; set; }

        static ConfigHelper()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();
        }

        /// <summary>
        /// Get the value based on the key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public static string Get(string key)
        {
            return Configuration[key];
        }

        /// <summary>
        /// Get the ConfigurationSection based on the key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>ConfigurationSection</returns>
        public static IConfigurationSection GetSection(string key)
        {
            return Configuration.GetSection(key);
        }
    }
}
