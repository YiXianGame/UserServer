using Make.MODEL;
using Material.Entity;
using Material.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.RPC.Request
{
    public interface UserRequest
    {
        public void AddHp(Token token, User user);
        public void SetSkillCardUpdate(Token token, long timestamp);

        public void SetFriendUpdate(Token token, long timestamp);

    }
}
