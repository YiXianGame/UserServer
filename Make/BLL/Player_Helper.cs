using Make.MODEL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Make.BLL
{
    static public class Player_Helper
    {
        public static string Int_To_String(int value)
        {
            if (value > 0) return "获得" + value.ToString() + "点";
            else return "损失" + (-value).ToString() + "点";
        }

    }
}
