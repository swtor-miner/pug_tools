using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TorLib
{
    public class Assets : IDisposable
    {
        //public static string assetPath = @"C:\Program Files (x86)\Electronic Arts\BioWare\Star Wars - The Old Republic\Assets";
        public string assetPath = @"C:\2.4PTS\Assets";
        public List<Library> libraries;
        public List<string> loadedFileGroups;
        private readonly System.Text.RegularExpressions.Regex fileNameParse = new System.Text.RegularExpressions.Regex("swtor_(?:test_)?(.*)_1");
        private bool disposed = false;

        //private static readonly string[] libraryNames = {"main","system","locale_en_us"};
        //private static readonly string[] libraryNames = { "main", "system" };

        //private static readonly string[] libraryNames = { "en-us_global", "main_gamedata", "main_gfx", "main_global", "main_systemgenerated_gom", "main_zed", "main_area_alderaan", "main_area_balmorra", "main_area_belsavis", "main_area_corellia", "main_area_coruscant", "main_area_dromund_kaas", "main_area_epsilon", "main_area_hoth", "main_area_hutta", "main_area_ilum", "main_area_korriban", "main_area_misc", "main_area_nar_shaddaa", "main_area_open_worlds", "main_area_ord_mantell", "main_area_quesh", "main_area_raid", "main_area_taris", "main_area_tatooine", "main_area_tython", "main_area_voss", "main_areadat", "main_areadat_epsilon" };

        public Assets(string _assetPath)
        {
            assetPath = _assetPath;
            icons = new Icons(this);
            loadedFileGroups = new List<string>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                foreach (Library lib in libraries)
                {
                    lib.Dispose();
                }
                libraries.Clear();
            }
            disposed = true;
        }

        ~Assets()
        {
            Dispose(false);
        }


        /*
        private bool Loaded { get; set; }

        
        private void Load()
        {
            bool isPtr = false;
            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["usePtrData"], out isPtr);
            Load(isPtr);
        }
         */

        public void Load(bool isPtr)
        {
            //if (Loaded) return;

            System.Configuration.ConfigurationManager.AppSettings["usePtrData"] = isPtr.ToString();
            //string language = System.Configuration.ConfigurationManager.AppSettings["language"] ?? "en-us";

            libraries = new List<Library>();
            //foreach (var libName in libraryNames)
            //{
            //    var lib = new Library(libName, assetPath);
            //    libraries.Add(lib);
            //}

            LoadAssetFiles("main", isPtr);
            LoadAssetFiles("en-us", isPtr);
            LoadAssetFiles("fr-fr", isPtr);
            LoadAssetFiles("de-de", isPtr);

            //beta
            LoadAssetFiles("locale_en_us", isPtr);
            LoadAssetFiles("system", isPtr);

            if (libraries.Count == 0)
            {
                if (isPtr == false)
                {
                    Load(true);
                }
                else
                {
                    throw new Exception("Could not find asset files!");
                }
            }

            //Loaded = true;
        }

        /*
        public void Unload()
        {
            if (Loaded)
            {
                libraries = null;
                Loaded = false;
                icons.Flush();
            }
        }
         */

        public Icons icons;

        private void LoadAssetFiles(string fileGroup, bool isPtr)
        {
            string searchPattern;
            if (isPtr)
            {
                searchPattern = string.Format("swtor_test_{0}_*_1.tor", fileGroup);
            }
            else
            {
                searchPattern = string.Format("swtor_{0}_*_1.tor", fileGroup);
            }

            var assetFilePaths = System.IO.Directory.GetFiles(assetPath, searchPattern, System.IO.SearchOption.TopDirectoryOnly);

            if (assetFilePaths.Length == 0)
            {
                searchPattern = string.Format("red_{0}_*.tor", fileGroup);
                assetFilePaths = System.IO.Directory.GetFiles(assetPath, searchPattern, System.IO.SearchOption.TopDirectoryOnly);
            }
            if (assetFilePaths.Length > 0)
            {
                loadedFileGroups.Add(fileGroup);
            }

            foreach (var assetFilePath in assetFilePaths)
            {
                var assetFileName = System.IO.Path.GetFileNameWithoutExtension(assetFilePath);
                var match = fileNameParse.Match(assetFileName);
                if (match.Success)
                {
                    string libName = match.Groups[1].Value;
                    var lib = new Library(libName, assetPath);
                    libraries.Add(lib);
                }
                else
                {
                    System.Text.RegularExpressions.Regex redNameParse = new System.Text.RegularExpressions.Regex("red_(?:test_)?(.*)");
                    match = redNameParse.Match(assetFileName);
                    if (match.Success)
                    {
                        string libName = match.Groups[1].Value;
                        var lib = new Library(libName, assetPath);
                        libraries.Add(lib);
                    }
                }
            }
        }

        //public static File FindFileByFqn(string fqn, string ext)
        //{
        //    string filePath = String.Format("/resources/server/{0}.{1}", fqn.Replace('.', '/'), ext);
        //    return FindFile(filePath);
        //}

        public File FindFile(string path)
        {
            //if (!Loaded) { Load(); }

            if (path == null) return null;
            // path = String.Format("/resources{0}", path.Replace('\\', '/'));
            path = path.Replace('\\', '/');

            File result = null;
            foreach (var lib in libraries)
            {
                result = lib.FindFile(path);
                if (result != null)
                {
                    return result;
                }
            }

            return result;
        }

        public bool HasFile(string path)
        {
            return FindFile(path) != null;
        }
    }
}
