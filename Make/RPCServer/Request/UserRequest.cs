using Material.Entity;
using Material.RPCServer.Annotation;
using Material.RPCServer.TCP_Async_Event;
using System.Collections.Generic;

namespace Make.RPCServer.Request
{
    public interface UserRequest
    {
        [RPCMethod]
        public void SetCardRepositoryUpdate(User user, long timestamp);
        [RPCMethod]
        public void SetFriendUpdate(User user, long timestamp);
        

    }
}
