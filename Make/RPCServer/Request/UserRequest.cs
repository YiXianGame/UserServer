using Material.Entity;
using System.Collections.Generic;
using Material.EtherealS.Annotation;

namespace Make.RPCServer.Request
{
    public interface UserRequest
    {
        [RPCRequest]
        public void SetFriendUpdate(User user, long timestamp);
        [RPCRequest]
        public void RefreshRepositorySkillCards(User user,long timestamp, List<SkillCard> skillCards,List<CardItem> cardItems);
        [RPCRequest]
        public bool Hello(long id, string message);

        [RPCRequest]
        public void SendMessage(long id,string message);
    }
}
