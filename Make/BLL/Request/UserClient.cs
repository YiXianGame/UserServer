using Make.MODEL;
using Material.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.BLL.Server
{
    public interface UserClient
    {
        public void AddHp(Token token, User user);
    }
}
