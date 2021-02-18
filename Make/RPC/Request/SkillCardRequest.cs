using Make.Model;

namespace Make.RPC.Request
{
    public interface SkillCardRequest
    {
        public void SyncSkillCardUpdate(UserToken token, long timestamp);
    }
}
