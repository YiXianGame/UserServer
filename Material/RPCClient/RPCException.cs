using System;

namespace Material.RPCClient
{
    class RPCException : Exception
    {
        public RPCException(string message) : base(message)
        {

        }
    }
}
