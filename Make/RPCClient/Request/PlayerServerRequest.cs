using Material.Entity;
using Material.Entity.Match;
using System.Collections.Generic;
using Material.EtherealC.Attribute;

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
