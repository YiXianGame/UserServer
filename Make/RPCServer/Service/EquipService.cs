using EtherealS.Attribute;
using EtherealS.Extension.Authority;
using Material.Entity;
using Material.Entity.Match;
using System.Collections.Generic;

namespace Make.RPCServer.Service
{
    public class EquipService : IAuthoritable
    {
        public object Authority { get => 1; set { } }
        [RPCService]
        public void SwitchCardGroup(User user, CardGroup cardGroup)
        {
            if (!user.Confirm && user.Squad != null && user.Team != null && user.TeamGroup != null)
            {
                user.CardGroup = cardGroup;
                foreach (MatchTeam team in user.TeamGroup.Items)
                {
                    foreach (MatchSquad squad in team.Items)
                    {
                        foreach (User player in squad.Items)
                        {
                            Core.EquipRequest.SwitchCardGroup(player, user.Id, cardGroup);
                        }
                    }
                }
            }
        }
        [RPCService]
        public bool ConfirmCardGroup(User user)
        {
            if (!user.Confirm && user.Squad != null && user.Team != null && user.TeamGroup != null)
            {
                if (user.CardGroup != null)
                {
                    user.Confirm = true;
                    ++user.TeamGroup.ConfirmCount;
                    if (user.TeamGroup.ConfirmCount == user.TeamGroup.Count)
                    {
                        List<Team> _teams = new List<Team>();
                        foreach (MatchTeam team in user.TeamGroup.Items)
                        {
                            Team _team = new Team();
                            foreach (MatchSquad squad in team.Items)
                            {
                                foreach (User item in squad.Items)
                                {
                                    Player player = new Player();
                                    player.SetAttribute(item);
                                    _team.Teammates.Add(player.Id, player);
                                }
                            }
                            _teams.Add(_team);
                        }
                        if (Core.PlayerServerRequest.CreateRoom(_teams, user.Squad.RoomType.ToString()))
                        {
                            foreach (MatchTeam team in user.TeamGroup.Items)
                            {
                                foreach (MatchSquad squad in team.Items)
                                {
                                    foreach (User player in squad.Items)
                                    {
                                        Core.EquipRequest.ConnectPlayerServer(player, Core.Config.PlayerServerConfig.Ip, Core.Config.PlayerServerConfig.Port);
                                    }
                                }
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
