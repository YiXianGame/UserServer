using Material.Entity;
using Material.RPCServer.Annotation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.RPCServer.Request
{
    public interface ReadyRequest
    {
        [RPCRequest]
        public void StartMatch(User user);
        [RPCRequest]
        public void MatchSuccess(User user, List<User> group_1, List<User> group_2, int idx, string hostname, string port, string secretKey);
        [RPCRequest]
        public void RefreshSquad(User user, List<User> users);
        [RPCRequest]
        public void InviteSquad(User user, User inviter, string secretKey);
        [RPCRequest]
        public void CancelMatch(User user);
        [RPCRequest]
        public void SwitchCardGroup(long id,bool isTeammates,CardGroup cardGroup);
    }
}
