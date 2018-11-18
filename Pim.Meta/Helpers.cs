using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pim.Meta
{
    internal static class Helpers
    {
        public static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        public static string ToSentenceCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            str = char.ToUpper(str[0]) + str.Substring(1);

            return Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
        }
    }
}
