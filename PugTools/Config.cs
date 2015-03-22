using System;
using System.Configuration;
using System.IO;

namespace tor_tools
{
    public static class Config
    {
        public static string AssetsPath = ".";
        public static string ExtractPath = ".";
        public static string PrevAssetsPath = ".";
        public static string ExtractAssetsPath = ".";
        public static bool CrossLinkDOM = false;
        public static Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static void Load()
        {
            string str = Config.configFile.AppSettings.Settings["AssetsPath"].Value;
            if (str != null)
                if (Directory.Exists(str))//Load from config, if directory exists
                    AssetsPath = str;
                //otherwise check some default directories
                else if (Directory.Exists("c:\\Program Files (x86)\\EA\\BioWare\\Star Wars - The Old Republic\\Assets\\"))
                    AssetsPath = "c:\\Program Files (x86)\\EA\\BioWare\\Star Wars - The Old Republic\\Assets\\";
                else if (Directory.Exists("C:\\Program Files (x86)\\Electronic Arts\\BioWare\\Star Wars-The Old Republic\\Assets\\"))
                    AssetsPath = "C:\\Program Files (x86)\\Electronic Arts\\BioWare\\Star Wars-The Old Republic\\Assets\\";
                else
                    AssetsPath = str;
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
            str = Config.configFile.AppSettings.Settings["PrevAssetsPath"].Value;
            if (str != null)
                PrevAssetsPath = str;
            str = Config.configFile.AppSettings.Settings["CrossLinkDOM"].Value;
            if (str != null)
                CrossLinkDOM = Convert.ToBoolean(str);
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
        }
        public static void Save()
        {
            string str = Config.AssetsPath;
            if (str != null)
                Config.configFile.AppSettings.Settings["AssetsPath"].Value = str;
            str = Config.ExtractPath;
            if (str != null)
                Config.configFile.AppSettings.Settings["ExtractPath"].Value = str;
            str = Config.PrevAssetsPath;
            if (str != null)
                Config.configFile.AppSettings.Settings["PrevAssetsPath"].Value = str;
            str = Config.CrossLinkDOM.ToString();
            if (str != null)
                Config.configFile.AppSettings.Settings["CrossLinkDOM"].Value = str;
            str = Config.ExtractAssetsPath;
            if (str != null)
                Config.configFile.AppSettings.Settings["ExtractAssetsPath"].Value = str;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
