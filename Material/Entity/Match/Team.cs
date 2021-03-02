using Material.Model.MatchSystem.Interface;
using System.Collections.Generic;

namespace Material.Entity.Match
{
    public class Team:Model.MatchSystem.Entity.BaseTeam<Squad>
    {
        public Team()
        {
            this.startMatchTime = Utils.TimeStamp.Now();
        }
        public override bool Add(Squad item)
        {
            if (base.Add(item))
            {
                foreach (User user in item.Items)
                {
                    user.Team = this;
                }
                return true;
            }
            else return false;
        }

        public override bool Remove(Squad item)
        {
            if (base.Remove(item))
            {
                foreach (User user in item.Items)
                {
                    user.Team = null;
                }
                return true;
            }
            else return false;
        }
    }
}
