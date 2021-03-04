using System.Collections.Generic;

namespace Material.Entity.Match
{
    public class MatchTeam : Model.MatchSystem.Entity.BaseTeam<MatchSquad>
    {
        List<Team> teams;
        public MatchTeam()
        {
            this.startMatchTime = Utils.TimeStamp.Now();
        }

        public List<Team> Teams { get => teams; set => teams = value; }

        public override bool Add(MatchSquad item)
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

        public override bool Remove(MatchSquad item)
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
