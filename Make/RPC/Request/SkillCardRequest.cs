using Make.MODEL;
using Material.Entity;
using Material.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.RPC.Request
{
    public interface SkillCardRequest
    {
        public void SyncSkillCardUpdate(Token token, long timestamp);
    }
}
