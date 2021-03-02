using System;

namespace Material.RPCServer.Annotation
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RPCRequest : Attribute
    {
        private string paramters = null;

        public string Paramters { get => paramters; set => paramters = value; }
    }
}
