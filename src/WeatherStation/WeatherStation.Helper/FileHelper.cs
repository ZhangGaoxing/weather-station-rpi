using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WeatherStation.Helper
{
    public class FileHelper
    {
        public static string FileToBase64(string path)
        {
            string base64 = "";

            using (FileStream filestream = new FileStream(path, FileMode.Open))
            {
                byte[] buffer = new byte[filestream.Length];

                filestream.Read(buffer, 0, buffer.Length);
                base64 = Convert.ToBase64String(buffer);
            }

            return base64;
        }

        public static void Base64ToFile(string base64, string path)
        {
            byte[] buffer = Convert.FromBase64String(base64);

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
            }
        }
    }
}
