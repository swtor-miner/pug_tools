using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorLib;

namespace GomLib
{
    public class DomHandler
    {
        private static readonly DomHandler instance = new DomHandler();
        DataObjectModel currentData;
        bool currentLoaded = false;

        DataObjectModel previousData;
        bool previousLoaded = false;

        static DomHandler()
        {
        }

        DomHandler()
        {

        }

        public static DomHandler Instance
        {
            get
            {
                return instance;
            }
        }

        public void DestroyCurrent()
        {

        }

        public DataObjectModel GetCurrentDOM(Assets assets = null)
        {
            if (currentLoaded)
                return currentData;
            else
            {
                if (assets != null)
                {
                    currentData = new DataObjectModel(assets);
                    currentData.Load();
                    currentLoaded = true;
                    return currentData;
                }
                else
                    throw new ArgumentException("No list of assests were provided");
            }
        }

        public DataObjectModel GetPreviousDOM(Assets assets = null)
        {
            if (previousLoaded)
                return previousData;
            else
            {
                if (assets != null)
                {
                    previousData = new DataObjectModel(assets);
                    previousData.Load();
                    previousLoaded = true;
                    return previousData;
                }
                else
                    throw new ArgumentException("No list of assests were provided");
            }
        }

        public void UnloadCurrentDOM()
        {
            if (currentLoaded)
            {
                currentData.Dispose();
                currentData = null;
                currentLoaded = false;
                GC.Collect();
            }

        }

        public void UnloadPreviousDOM()
        {
            if (previousLoaded)
            {
                previousData.Dispose();
                previousData = null;
                previousLoaded = false;
                GC.Collect();
            }
        }

        public void UnloadAllDOM()
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
