using Newtonsoft.Json;

namespace Material.Entity.Match
{
    public class MatchTeamGroup:Model.MatchSystem.Entity.BaseTeamGroup<MatchTeam>
    {
        int confirmCount;
        public int ConfirmCount { get => confirmCount; set => confirmCount = value; }

        public override bool Add(MatchTeam item)
        {
            if (base.Add(item))
            {
                foreach (MatchSquad squad in item.Items)
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

        public override bool Remove(MatchTeam item)
        {
            if (base.Add(item))
            {
                foreach (MatchSquad squad in item.Items)
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
