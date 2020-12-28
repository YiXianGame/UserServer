using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Material
{
    public static class Adventure_Helper
    {
        public static string Get_Adventure_ID(int ID)
        {
            string filepath = App.directory + "\\奇遇\\" + ID.ToString() + ".json";
            if (!File.Exists(filepath)) return null;
            string json = (File.ReadAllText(filepath));
            return json;
        }
    }
}
