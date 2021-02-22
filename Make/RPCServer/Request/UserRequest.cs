using Material.RPCServer.TCP_Async_Event;
using System.Collections.Generic;

namespace Make.RPCServer.Request
{
    public interface UserRequest
    {
        public void MatchSucess(BaseUserToken token, List<long> group_1,List<long> group_2,int idx,string hostname, string port, string hash);
        public void SetSkillCardUpdate(BaseUserToken token, long timestamp);

        public void SetFriendUpdate(BaseUserToken token, long timestamp);

        public void CancelMatch(BaseUserToken token);

    }
}
