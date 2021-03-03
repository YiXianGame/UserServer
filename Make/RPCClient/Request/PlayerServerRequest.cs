using Material.Entity;
using Material.RPCClient.Annotation;
using System.Collections.Generic;

namespace Make.RPCClient.Request
{
    public interface PlayerServerRequest
    {
        [RPCRequest]
        public string CreateRoom(List<long> redTeam, List<long> blueTeam,Room.RoomType roomType);
    }
}
