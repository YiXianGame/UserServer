using Make.Model;
using Material.Entity;
using Material.RPCServer.Annotation;
using System.Collections.Generic;

namespace Make.RPCServer.Adapt
{
    public class SkillCardAdapt
    {
        [RPCMethod]
        public List<SkillCard> Sync(User user, long timestamp)
        {
            if (Core.Config.SkillCardUpdate.Equals(timestamp))
            {
                return null;
            }
            else
            {
                Core.SkillCardRequest.SetSkillCardUpdate(user, Core.Config.SkillCardUpdate);
                return new List<SkillCard>(Core.SkillCardByID.Values);
            }
        }
        [RPCMethod]
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
