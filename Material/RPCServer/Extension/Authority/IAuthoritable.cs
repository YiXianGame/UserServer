using System;
using System.Collections.Generic;
using System.Text;

namespace Material.RPCServer.Extension.Authority
{
    public interface IAuthoritable
    {
        public object Authority { get; set; }
    }
}
