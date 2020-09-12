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
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["usePtrData"], out bool isPtr);

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
