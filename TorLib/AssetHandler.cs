using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorLib
{
    public class AssetHandler : IDisposable
    {
        private static readonly AssetHandler instance = new AssetHandler();
        Assets currentData;
        Assets previousData;
        bool currentLoaded;
        bool previousLoaded;
        bool disposed = false;

        //public Dictionary<string, Assets> loadedData = new Dictionary<string, Assets>();

        static AssetHandler()
        {

        }

        AssetHandler()
        {

        }

        public static AssetHandler Instance
        {
            get
            {
                return instance;
            }
        }

        public void DestroyCurrent()
        {
            currentLoaded = false;
            currentData = null;
            GC.Collect();
        }

        public Assets GetCurrentAssets(string path = null, bool isPTR = false)
        {
            if (currentLoaded)
            {
                return currentData;
            }
            else
            {
                if (path != null)
                {
                    currentData = new Assets(path);
                    currentData.Load(isPTR);
                    currentLoaded = true;

                    return currentData;
                }
                else
                    throw new ArgumentException("No path to the assests was provided");
            }
        }

        public Assets GetPreviousAssets(string path = null, bool isPTR = false)
        {
            if (previousLoaded)
            {
                return previousData;
            }
            else
            {
                if (path != null)
                {
                    previousData = new Assets(path);
                    previousData.Load(isPTR);
                    previousLoaded = true;
                    HashDictionaryInstance.Instance.Unload();
                    return previousData;
                }
                else
                    throw new ArgumentException("No path to the assests were provided");
            }
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
                currentData.Dispose();
                previousData.Dispose();
            }
            disposed = true;
        }

        ~AssetHandler()
        {
            Dispose(false);
        }

        public void UnloadCurrentAssets()
        {
            if (currentLoaded)
            {
                currentData.Dispose();
                currentData = null;
                currentLoaded = false;
            }
        }

        public void UnloadPreviousAssets()
        {
            if (previousLoaded)
            {
                previousData.Dispose();
                previousData = null;
                previousLoaded = false;
            }
        }

        public void UnloadAllAssets()
        {
            if (currentLoaded)
            {
                currentData.Dispose();
                currentData = null;
                currentLoaded = false;
            }

            if (previousLoaded)
            {
                previousData.Dispose();
                previousData = null;
                previousLoaded = false;
            }
            GC.Collect();
        }
    }
}
