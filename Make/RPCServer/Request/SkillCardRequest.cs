using Make.Model;

namespace Make.RPCServer.Request
{
    public interface SkillCardRequest
    {
        public void SyncSkillCardUpdate(UserToken token, long timestamp);
    }
}
