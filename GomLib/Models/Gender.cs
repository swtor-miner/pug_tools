using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum Gender
    {
        None,
        Male,
        Female
    }

    public static class GenderExtensions
    {
        public static Gender ToGender(this string str)
        {
            if (String.IsNullOrEmpty(str)) { return Gender.None; }
            str = str.ToLower();

            switch (str)
            {
                case "chrgendermale":
                case "male":
                    return Gender.Male;
                case "chrgenderfemale":
                case "female":
                    return Gender.Female;
                case "chrgendernone":
                    return Gender.None;
                default: throw new InvalidOperationException("Unknown Gender: " + str);
            }
        }

        public static Gender ToGender(this ScriptEnum val)
        {
            if (val == null) { return Gender.Male; }
            return ToGender(val.ToString());
        }
    }
}
