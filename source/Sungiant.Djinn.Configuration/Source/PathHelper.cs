using System;
using System.IO;

namespace Sungiant.Djinn.Configuration
{
    class PathHelper
    {
        public static String ResolveDirectoryForUser (String str)
        {
            if (str.IndexOf ("~/") == 0)
            {
                str = Path.Combine(
                    Environment.GetEnvironmentVariable("HOME"),
                    str.Substring(2, str.Length - 2));
            }
            
            return str;
        }
    }
}

