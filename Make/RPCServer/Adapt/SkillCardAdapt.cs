using Make.Model;
using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.RPCServer.Adapt
{
    public class SkillCardAdapt
    {
        public static List<SkillCard> Sync(UserToken token, long timestamp)
        {
            if (Core.Config.SkillCardUpdate.Equals(timestamp))
            {
                return null;
            }
            else
            {
                Core.SkillCardRequest.SyncSkillCardUpdate(token,Core.Config.SkillCardUpdate);
                return new List<SkillCard>(Core.SkillCardByID.Values);
            }
        }
        public static List<SkillCard> Query(UserToken token, List<long> skillCardIds)
        {
            List<SkillCard> skillCards = new List<SkillCard>();
            foreach(long item in skillCardIds)
            {
                SkillCard card = Core.Repository.SkillCardRepository.QuerySync(item);
                skillCards.Add(card);
            }
            if (skillCardIds.Count != skillCards.Count) return null;
            else return skillCards;
        }
    }
}
