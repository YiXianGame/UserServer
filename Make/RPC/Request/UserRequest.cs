using Make.Model;
using Material.Entity;
using Material.TCP_Async_Event;
using System.Collections.Generic;

namespace Make.RPC.Request
{
    public interface UserRequest
    {
        public void MatchSucess(UserToken token,List<long> users,string hostname,string port,string hash);
        public void SetSkillCardUpdate(UserToken token, long timestamp);

        public void SetFriendUpdate(UserToken token, long timestamp);

    }
}
