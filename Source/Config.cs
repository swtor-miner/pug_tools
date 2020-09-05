using System;
using System.Configuration;
using System.IO;

namespace PugTools
{
    public static class Config
    {
        public static string AssetsPath = ".";

        public static bool AssetsUsePTS = false;

        public static string PrevAssetsPath = ".";

        public static bool PrevAssetsUsePTS = false;

        public static string ExtractPath = ".";

        public static string ExtractAssetsPath = ".";

        public static bool CrossLinkDOM = false;

        public static Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static void Load()
        {
            // Path to the asset files
            string str = Config.configFile.AppSettings.Settings["AssetsPath"].Value;
            if (str != null)
                if (Directory.Exists(str)) //Load from config, if directory exists
                    AssetsPath = str;
                //otherwise check some default directories
                else if (Directory.Exists("C:\\Program Files (x86)\\EA\\BioWare\\Star Wars - The Old Republic\\Assets\\"))
                    AssetsPath = "C:\\Program Files (x86)\\EA\\BioWare\\Star Wars - The Old Republic\\Assets\\";
                else if (Directory.Exists("C:\\Program Files (x86)\\Electronic Arts\\BioWare\\Star Wars - The Old Republic\\Assets\\"))
                    AssetsPath = "C:\\Program Files (x86)\\Electronic Arts\\BioWare\\Star Wars - The Old Republic\\Assets\\";
                else if (Directory.Exists("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Star Wars - The Old Republic\\Assets\\"))
                    AssetsPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Star Wars - The Old Republic\\Assets\\";
                else
                    AssetsPath = str;

            // Load PTS assets if checked
            str = Config.configFile.AppSettings.Settings["AssetsUsePTS"].Value;
            if (str != null)
                AssetsUsePTS = Convert.ToBoolean(str);

            // Path to the previous asset files
            str = Config.configFile.AppSettings.Settings["PrevAssetsPath"].Value;
            if (str != null)
                if (Directory.Exists(str)) //Load from config, if directory exists
                    PrevAssetsPath = str;
                //otherwise check some default directories
                else if (Directory.Exists("C:\\Program Files (x86)\\EA\\BioWare\\Star Wars - The Old Republic\\Assets\\"))
                    PrevAssetsPath = "C:\\Program Files (x86)\\EA\\BioWare\\Star Wars - The Old Republic\\Assets\\";
                else if (Directory.Exists("C:\\Program Files (x86)\\Electronic Arts\\BioWare\\Star Wars - The Old Republic\\Assets\\"))
                    PrevAssetsPath = "C:\\Program Files (x86)\\Electronic Arts\\BioWare\\Star Wars - The Old Republic\\Assets\\";
                else if (Directory.Exists("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Star Wars - The Old Republic\\Assets\\"))
                    PrevAssetsPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Star Wars - The Old Republic\\Assets\\";
                else
                    PrevAssetsPath = str;

            // Load PTS assets if checked
            str = Config.configFile.AppSettings.Settings["PrevAssetsUsePTS"].Value;
            if (str != null)
                PrevAssetsUsePTS = Convert.ToBoolean(str);

            // Path to the extract files
            str = Config.configFile.AppSettings.Settings["ExtractPath"].Value;
            if (str != null)
            {
                if (!str.EndsWith("\\"))
                {
                    ExtractPath = str + "\\";
                }
                else
                {
                    ExtractPath = str;
                }
            }

            // Path Where to Extract Assets
            str = Config.configFile.AppSettings.Settings["ExtractAssetsPath"].Value;
            if (str != null)
            {
                if (!str.EndsWith("\\"))
                {
                    ExtractAssetsPath = str + "\\";
                }
                else
                {
                    ExtractAssetsPath = str;
                }
            }

            // Cross Link DOM
            str = Config.configFile.AppSettings.Settings["CrossLinkDOM"].Value;
            if (str != null)
                CrossLinkDOM = Convert.ToBoolean(str);
        }
        public static void Save()
        {
            // Path to the asset files
            string str = Config.AssetsPath;
            if (str != null)
                Config.configFile.AppSettings.Settings["AssetsPath"].Value = str;

            // Load PTS assets if checked
            str = Config.AssetsUsePTS.ToString();
            if (str != null)
                Config.configFile.AppSettings.Settings["AssetsUsePTS"].Value = str;

            // Path to the previous asset files
            str = Config.PrevAssetsPath;
            if (str != null)
                Config.configFile.AppSettings.Settings["PrevAssetsPath"].Value = str;

            // Load PTS assets if checked
            str = Config.PrevAssetsUsePTS.ToString();
            if (str != null)
                Config.configFile.AppSettings.Settings["PrevAssetsUsePTS"].Value = str;

            // Path to the extract files
            str = Config.ExtractPath;
            if (str != null)
                Config.configFile.AppSettings.Settings["ExtractPath"].Value = str;

            // Path to the extract files
            str = Config.ExtractAssetsPath;
            if (str != null)
                Config.configFile.AppSettings.Settings["ExtractAssetsPath"].Value = str;

            // Cross Link DOM
            str = Config.CrossLinkDOM.ToString();
            if (str != null)
                Config.configFile.AppSettings.Settings["CrossLinkDOM"].Value = str;

            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
