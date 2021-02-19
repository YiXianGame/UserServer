using System;

namespace Material.RPCServer
{
    class RPCException : Exception
    {
        public RPCException(string message) : base(message)
        {

        }
    }
}
