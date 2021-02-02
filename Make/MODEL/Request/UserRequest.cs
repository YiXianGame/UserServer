using Material.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.MODEL.Server
{
    public interface UserRequest
    {
        public void AddHp(Token token, User user);
    }
}
