using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TorLib
{
    public static class GameInfo
    {
        public static readonly Patch CurrentVersion;

        static GameInfo()
        {
            bool isPtr = false;
            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["usePtrData"], out isPtr);

            if (isPtr)
            {
                CurrentVersion = Patch.v1_1_5; // PTR
            }
            else
            {
                CurrentVersion = Patch.v1_1_4; // Live
            }
        }
    }
}
