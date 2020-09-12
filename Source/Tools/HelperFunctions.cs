﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PugTools
{
    class HelperFunctions
    {
    }

    public static class ObjectExtensions
    {
        public static string NullSafeToString(this object obj)
        {
            return obj != null ? obj.ToString() : string.Empty;
        }
    }
}
