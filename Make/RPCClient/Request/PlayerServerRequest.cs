using Material.Entity;
using Material.Entity.Game;
using System.Collections.Generic;

namespace Make.RPCClient.Request
{
    public interface PlayerServerRequest
    {
        public string CreateRoom(List<User> team_a,List<User> team_b,Room.RoomType roomType,string token);
    }
}
