using Material.Entity;
using Material.Entity.Match;
using System.Collections.Generic;

namespace Make.BLL
{
    public class MatchSystemHelper
    {
        public static void SoloGroupMatchSystem_MatchSuccessEvent(List<MatchTeamGroup> teamGroups)
        {
            foreach (MatchTeamGroup teams in teamGroups)
            {
                List<Team> _teams = new List<Team>();
                foreach (MatchTeam team in teams.Items)
                {
                    Team _team = new Team();
                    foreach (MatchSquad squad in team.Items)
                    {
                        foreach (User user in squad.Items)
                        {
                            Player player = new Player();
                            player.SetAttribute(user);
                            _team.Teammates.Add(player.Id, player);
                        }
                    }
                    _teams.Add(_team);
                }

                foreach (MatchTeam team in teams.Items)
                {
                    foreach (MatchSquad squad in team.Items)
                    {
                        foreach (User user in squad.Items)
                        {
                            Core.ReadyRequest.MatchSuccess(user, _teams);
                        }
                    }
                }
            }
        }

    }
}
