using Material.Entity;
using Material.Entity.Match;
using System.Collections.Generic;
using System.Threading.Tasks;
using Material.EtherealS.Annotation;
using Material.EtherealS.Extension.Authority;

namespace Make.RPCServer.Service
{
    public class ReadyService :IAuthoritable
    {
        public object Authority { get => 1; set { } }

        [RPCService]
        public List<User> EnterSquad(User user, long id, string secretKey)
        {
            if (user.GetToken(id, out User host))
            {
                if (host.Squad != null)
                {
                    if (user.SumRank == -1)
                    {
                        Task<User> task = Core.Repository.UserRepository.Query_AttributeById(user.Id);
                        task.Wait();
                        user.SumRank = task.Result.Lv;
                    }

                    if (host.Squad.SecretKey.Equals(secretKey) && host.Squad.Add(user))
                    {
                        List<User> users = new List<User>(host.Squad.Items);
                        foreach (User item in host.Squad.Items)
                        {
                            Core.ReadyRequest.RefreshSquad(item, users);
                        }
                        return users;
                    }
                    else return null;
                }
                else return null;
            }
            else return null;
        }
        [RPCService]
        public bool InviteSquad(User user, long id)
        {
            if (user.Squad != null)
            {
                if (user.GetToken(id, out User invited))
                {
                    Core.ReadyRequest.InviteSquad(invited, user, user.Squad.SecretKey);
                    return true;
                }
                else return false;
            }
            else return false;
        }
        [RPCService]
        public void StartMatch(User user)
        {
            if (user.Squad != null)
            {
                if (user.Squad.Captain == user && user.Squad.RoomType == Room.RoomType.Round_Solo)
                {
                    if (Core.SoloMatchSystem.Enter(user.Squad))
                    {
                        foreach (User player in user.Squad.Items)
                        {
                            Core.ReadyRequest.StartMatch(player);
                        }
                    }
                }
            }
        }
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
                            if (player.Team == user.Team)
                            {
                                Core.ReadyRequest.SwitchCardGroup(player, user.Id, true, cardGroup);
                            }
                            else Core.ReadyRequest.SwitchCardGroup(player, user.Id, false, cardGroup);
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
                if(user.CardGroup != null)
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
                        if(Core.PlayerServerRequest.CreateRoom(_teams, user.Squad.RoomType.ToString()))
                        {
                            foreach (MatchTeam team in user.TeamGroup.Items)
                            {
                                foreach (MatchSquad squad in team.Items)
                                {
                                    foreach (User player in squad.Items)
                                    {
                                        Core.ReadyRequest.ConnectPlayerServer(player, Core.Config.PlayerServerConfig.Ip, Core.Config.PlayerServerConfig.Port);
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
