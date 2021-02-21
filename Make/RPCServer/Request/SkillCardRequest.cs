using Material.RPCServer.TCP_Async_Event;

namespace Make.RPCServer.Request
{
    public interface SkillCardRequest
    {
        public void SyncSkillCardUpdate(BaseUserToken token, long timestamp);
    }
}
