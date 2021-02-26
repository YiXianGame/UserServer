using Make.Model;
using Material.Entity;
using Material.Entity.Match;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.BLL
{
    public class MatchSystemHelper
    {
        public static void SoloGroupMatchSystem_MatchSuccessEvent(List<TeamGroup> teamGroups)
        {
            foreach(TeamGroup teams in teamGroups)
            {
                if (teams.Items.Count == 2)
                {
                    List<List<User>> result = new List<List<User>>(2);
                    foreach (Team team in teams.Items)
                    {
                        List<User> users = new List<User>();
                        foreach (Squad squad in team.Items)
                        {
                            foreach (User user in squad.Items)
                            {
                                users.Add(user);
                            }
                        }
                        result.Add(users);
                    }
                    int idx = 0;
                    foreach (Team team in teams.Items)
                    {
                        foreach (Squad squad in team.Items)
                        {
                            foreach (User user in squad.Items)
                            {
                                Core.ReadyRequest.MatchSuccess(user, result[0], result[1], idx, "192.168.0.105", "28016", Material.Utils.SecretKey.Generate(10));
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
                            foreach (User user in squad.Items)
                            {
                                Core.ReadyRequest.CancelMatch(user);
                            }
                        }
                        idx++;
                    }
                }
            }
        }

    }
}
