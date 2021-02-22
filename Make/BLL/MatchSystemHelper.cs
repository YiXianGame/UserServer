using Make.Model;
using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.BLL
{
    public class MatchSystemHelper
    {
        public static void SoloGroupMatchSystem_MatchSucessEvent(List<TeamGroup> teamGroups)
        {
            foreach(TeamGroup teams in teamGroups)
            {
                if (teams.Items.Count == 2)
                {
                    List<List<long>> result = new List<List<long>>(2);
                    foreach (Team team in teams.Items)
                    {
                        List<long> users = new List<long>();
                        foreach (Squad squad in team.Items)
                        {
                            foreach (UserToken user in squad.Items)
                            {
                                users.Add(user.UserId);
                            }
                        }
                    }
                    int idx = 0;
                    foreach (Team team in teams.Items)
                    {
                        foreach (Squad squad in team.Items)
                        {
                            foreach (UserToken user in squad.Items)
                            {
                                Core.UserRequest.MatchSucess(user, result[0], result[1], idx, "192.168.0.105", "28016", "这里发一段HASH验证");
                            }
                        }
                        idx++;
                    }
                }
                else
                {
                    int idx = 0;
                    foreach (Team team in teams.Items)
                    {
                        foreach (Squad squad in team.Items)
                        {
                            foreach (UserToken user in squad.Items)
                            {
                                Core.UserRequest.CancelMatch(user);
                            }
                        }
                        idx++;
                    }
                }
            }
        }

        public static void SoloMatchSystem_MatchSucessEvent(List<Team> teamGroups)
        {
            foreach (Team team in teamGroups)
            {
                Core.SoloGroupMatchSystem.Add(team);
            }
            Core.SoloGroupMatchSystem.Start();
        }
    }
}
