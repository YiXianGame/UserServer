using Make.Model;
using Material.Entity;
using System.Collections.Generic;
using EtherealS.Attribute;
using EtherealS.Extension.Authority;

namespace Make.RPCServer.Service
{
    public class SkillCardService : IAuthoritable
    {
        public object Authority { get => 1; set { } }
        [RPCService]
        public List<SkillCard> Query(User user, List<long> skillCardIds)
        {
            List<SkillCard> skillCards = new List<SkillCard>();
            foreach (long item in skillCardIds)
            {
                SkillCard card = Core.Repository.SkillCardRepository.QuerySync(item);
                skillCards.Add(card);
            }
            if (skillCardIds.Count != skillCards.Count) return null;
            else return skillCards;
        }
    }
}
