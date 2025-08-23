using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_screenshot_ai
{
    public static class GlobalData
    {
        public static Dictionary<string , string> Synonyms { get; set; } = new Dictionary<string , string>();
        internal static string dbname { get; set; }

        internal static string JsonPath { get; set; }
    }
}
