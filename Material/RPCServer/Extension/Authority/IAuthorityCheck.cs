using System;
using System.Collections.Generic;
using System.Text;

namespace Material.RPCServer.Extension.Authority
{
    public interface IAuthorityCheck:IAuthoritable
    {
        public bool Check(IAuthoritable authoritable);
    }
}
