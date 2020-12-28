using Make.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Make.BLL
{
    public static class State_Helper
    {
        public static async Task Wait(int num)
        {
            await Task.Delay(num);
            return ;
        }
    }
}
