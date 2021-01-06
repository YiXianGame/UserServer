using Make.MODEL.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.MODEL.RPC.Request
{
    public interface ICommand
    {
        public void AddHp(Token token, long num);
    }
}
