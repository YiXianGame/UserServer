using Material.Entity;
using Material.RPCServer.Annotation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.RPCServer.Request
{
    public interface ReadyRequest
    {
        [RPCMethod]
        public void StartMatch(User user);
        [RPCMethod]
        public void MatchSuccess(User user, List<User> group_1, List<User> group_2, int idx, string hostname, string port, string secretKey);
        [RPCMethod]
        public void RefreshSquad(User user, List<User> users);
        [RPCMethod]
        public void InviteSquad(User user, User inviter, string secretKey);
        [RPCMethod]
        public void CancelMatch(User user);
    }
}
