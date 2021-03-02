using Material.Entity;
using Material.Entity.Game;
using Material.RPCClient.Annotation;
using System.Collections.Generic;

namespace Make.RPCClient.Request
{
    public interface PlayerServerRequest
    {
        [RPCRequest]
        public string CreateRoom(List<User> team_a,List<User> team_b,Room.RoomType roomType,string token);
    }
}
