using Material.Entity;
using System.Collections.Generic;
using Material.EtherealS.Annotation;

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
