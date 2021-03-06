using Material.Entity;
using Material.Entity.Match;
using Material.RPCClient.Annotation;
using System.Collections.Generic;

namespace Make.RPCClient.Request
{
    public interface PlayerServerRequest
    {
        [RPCRequest]
        public bool CreateRoom(List<Team> teams,string roomType);
    }
}
