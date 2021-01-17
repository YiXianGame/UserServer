using System;
using System.Collections.Generic;
using System.Text;

namespace Make.MODEL.RPC
{
    class RPCException : Exception
    {
        public RPCException(string message) : base(message)
        {

        }
    }
}
