using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Project_screenshot_ai
{
    public static class JsonHelper
    {
        public static List<QA> LoadFromJson(string filePath)
        {
            String json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<QA>>(json);
        }
    }
}
