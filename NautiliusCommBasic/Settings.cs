using System;
using System.IO;

namespace NautiliusCommBasic
{
    internal class Settings
    {
        public virtual Uri HostUrl { get; set; } = new Uri("http://test.com/s");
        public virtual Uri ClientUrl { get; set; } = new Uri("http://test.com/s");

        internal static Settings Load(string settingsFileLocation)
        {
            string settingsString;
            using (var fs = new StreamReader(File.OpenRead(settingsFileLocation)))
            {
                settingsString = fs.ReadToEnd();
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(settingsString);
        }
    }
}