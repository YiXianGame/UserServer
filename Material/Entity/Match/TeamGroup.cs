using Material.Model.MatchSystem.Interface;
using System.Collections.Generic;

namespace Material.Entity.Match
{
    public class TeamGroup:Model.MatchSystem.Entity.BaseTeamGroup<Team>
    {
        public override bool Add(Team item)
        {
            if (base.Add(item))
            {
                foreach (Squad squad in item.Items)
                {
                    foreach (User user in squad.Items)
                    {
                        user.TeamGroup = this;
                    }
                }
                return true;
            }
            else return false;
        }

        public override bool Remove(Team item)
        {
            if (base.Add(item))
            {
                foreach (Squad squad in item.Items)
                {
                    foreach (User user in squad.Items)
                    {
                        user.TeamGroup = null;
                    }
                }
                return true;
            }
            else return false;
        }
    }
}
