using Material.Entity;
using Material.Entity.Match;
using Material.RPCClient.Annotation;
using System.Collections.Generic;

namespace Make.RPCClient.Request
{
    public interface PlayerServerRequest
    {
        [RPCRequest]
        public string CreateRoom(List<Team> teams,Room.RoomType roomType);
    }
}
