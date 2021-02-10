using Material.Entity;
using Material.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.RPC.Server
{
    public class SkillCardServer
    {
        public static List<SkillCard> Test(Token token, List<SkillCard> list)
        {
            list[0].Name = "剑";
            return list;
        }
    }
}
