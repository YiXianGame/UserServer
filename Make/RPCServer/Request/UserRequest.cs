using Material.Entity;
using Material.RPCServer.TCP_Async_Event;
using System.Collections.Generic;

namespace Make.RPCServer.Request
{
    public interface UserRequest
    {
        public void SetSkillCardUpdate(User user, long timestamp);
        public void SetFriendUpdate(User user, long timestamp);
        public void CancelMatch(User user);
        public void MatchSuccess(User user, List<User> group_1, List<User> group_2, int idx, string hostname, string port, string secretKey);
        public void RefreshSquad(User user, List<User> users);

        public void InviteSquad(User user, User inviter,string secretKey);
    }
}
