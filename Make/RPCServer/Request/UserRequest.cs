using Make.Model;
using Material.Entity;
using System.Collections.Generic;

namespace Make.RPCServer.Request
{
    public interface UserRequest
    {
        public void MatchSucess(UserToken token,List<long> users,string hostname,string port,string hash);
        public void SetSkillCardUpdate(UserToken token, long timestamp);

        public void SetFriendUpdate(UserToken token, long timestamp);

    }
}
