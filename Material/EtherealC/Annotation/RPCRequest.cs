using System;

namespace Material.EtherealC.Annotation
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RPCRequest : Attribute
    {
        private string[] paramters = null;

        public string[] Paramters { get => paramters; set => paramters = value; }
    }
}
