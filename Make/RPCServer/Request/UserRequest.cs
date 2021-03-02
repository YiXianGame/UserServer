using Material.Entity;
using Material.RPCServer.Annotation;
using Material.RPCServer.TCP_Async_Event;
using System.Collections.Generic;

namespace Make.RPCServer.Request
{
    public interface UserRequest
    {
        [RPCRequest]
        public void SetCardRepositoryUpdate(User user, long timestamp);
        [RPCRequest]
        public void SetFriendUpdate(User user, long timestamp);
        

    }
}
