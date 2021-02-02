using System;
using System.Collections.Generic;
using System.Text;

namespace Material.RPC
{
    public class Error
    {
        int Code { get; set; }
        string Message { get; set; }
        string Data { get; set; }
        public override string ToString()
        {
            return "Code:" + Code 
                + " Message:" + Message 
                + " Data:" + Data + "\n";
        }
    }
}
