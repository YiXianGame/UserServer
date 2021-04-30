using EtherealS.Attribute;
using Material.Entity;

namespace Make.RPCServer.Request
{
    public interface SkillCardRequest
    {
        [RPCRequest]
        public void SetSkillCardUpdate(User user, long timestamp);


    }
}
