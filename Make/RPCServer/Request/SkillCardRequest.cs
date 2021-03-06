using Material.Entity;
using Material.EtherealS.Annotation;

namespace Make.RPCServer.Request
{
    public interface SkillCardRequest
    {
        [RPCRequest]
        public void SetSkillCardUpdate(User user, long timestamp);


    }
}
