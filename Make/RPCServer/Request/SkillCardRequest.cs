using Material.Entity;
using Material.RPCServer.TCP_Async_Event;

namespace Make.RPCServer.Request
{
    public interface SkillCardRequest
    {
        public void SetSkillCardUpdate(User user, long timestamp);


    }
}
