using System;
using Material.EtherealS.Extension.Authority;

namespace Material.EtherealS.Annotation
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RPCService : Attribute,IAuthoritable
    {
        private string[] paramters = null;
        public string[] Paramters { get => paramters; set => paramters = value; }

        public object authority = null;
        public object Authority { get => authority; set => authority = value; }
    }
}
