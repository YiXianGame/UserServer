using Material.Entity;
using System.Collections.Generic;
using EtherealC.Attribute;

namespace Make.RPCClient.Request
{
    public interface PlayerServerRequest
    {
        [RPCRequest]
        public bool CreateRoom(List<Team> teams,string roomType);
        [RPCRequest]
        public bool Login(string secretKey);
    }
}
