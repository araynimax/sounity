using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Sounity
{
    public class Config
    {
        #region "singleton stuff"
        private static Config instance = null;

        public static Config GetInstance()
        {
            if (instance == null) instance = new Config();

            return instance;
        }
        #endregion

        public Dictionary<string, string> config_dict = new Dictionary<string, string>();

        private Config()
        {
            ReloadConfig();
        }

        public void ReloadConfig()
        {
            var configFileContent = API.LoadResourceFile(API.GetCurrentResourceName(), "config.ini");

            if (configFileContent == null || configFileContent == "") return;

            ParseConfig(configFileContent);
        }

        private void ParseConfig(string configFileContent)
        {
            StringReader reader = new StringReader(configFileContent);

            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                line.Trim(); // remove whitespaces
                if (line.StartsWith(";") || line.StartsWith("#")) continue; // ignore comments (line starts with ; or #)
                if (line.Length == 0) continue; // ignore empty lines
                if (line.StartsWith("[")) continue; // ignore sections (i wont use them for getting config values)
                if (!line.Contains("=")) continue; // ignore "invalid" lines

                string[] splittedLine = line.Split('=');

                string key = splittedLine[0].Trim();
                string value = splittedLine[1].Trim();

                config_dict[key] = value;
            }
        }

        public bool Exists(string key)
        {
            return config_dict.ContainsKey(key);
        }

        public string Get(string key, string fallback)
        {
            if (!Exists(key)) return fallback;

            return config_dict[key];
        }

        public float Get(string key, float fallback)
        {
            if (!Exists(key)) return fallback;

            string value = config_dict[key];

            return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
        }

        public int Get(string key, int fallback)
        {
            if (!Exists(key)) return fallback;

            string value = config_dict[key];

            return int.Parse(value);
        }

        public bool Get(string key, bool fallback)
        {
            if (!Exists(key)) return fallback;

            string value = config_dict[key];

            return value.ToLower() == "true";
        }
    }
}
