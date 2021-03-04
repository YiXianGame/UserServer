using Material.Entity;
using Material.Entity.Match;
using Material.RPCServer.Annotation;
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
        [RPCRequest]
        public void SwitchCardGroup(User user,long id,bool isTeammates,CardGroup cardGroup);
        [RPCRequest]
        public void ConnectPlayerServer(User user,string ip,string port,string roomId);
    }
}
