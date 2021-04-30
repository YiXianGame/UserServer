using EtherealS.Attribute;
using Material.Entity;
using System.Collections.Generic;

namespace Make.RPCServer.Request
{
    public interface ReadyRequest
    {
        [RPCRequest]
        public void StartMatch(User user);
        [RPCRequest]
        public void MatchSuccess(User user, List<Team> teams);
        [RPCRequest]
        public void RefreshSquad(User user, List<User> users);
        [RPCRequest]
        public void InviteSquad(User user, User inviter, string secretKey);
        [RPCRequest]
        public void CancelMatch(User user);
    }
}
