using Material.Entity;
using Material.Entity.Match;
using Material.RPCServer.Annotation;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Make.RPCServer.Service
{
    public class ReadyService
    {

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
        public void SwitchCardGroup(User user,CardGroup cardGroup)
        {
            if (!user.Confirm && user.Squad != null && user.Team != null && user.TeamGroup != null)
            {
                user.CardGroup = cardGroup;
                foreach(Team team in user.TeamGroup.Items)
                {
                    foreach(Squad squad in team.Items)
                    {
                        foreach(User player in squad.Items)
                        {
                            if (player.Team == user.Team)
                            {
                                Core.ReadyRequest.SwitchCardGroup(user.Id, true, cardGroup);
                            }
                            else Core.ReadyRequest.SwitchCardGroup(user.Id, false, cardGroup);
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

                        List<long> redTeam = new List<long>();
                        List<long> blueTeam = new List<long>();
                        int idx = 0;
                        foreach (Team team in user.TeamGroup.Items)
                        {
                            foreach (Squad squad in team.Items)
                            {
                                foreach (User player in squad.Items)
                                {
                                    if (idx == 0) redTeam.Add(player.Id);
                                    else blueTeam.Add(player.Id);
                                }
                            }
                            ++idx;
                        }
                        string secretKey = Core.PlayerServerRequest.CreateRoom(redTeam, blueTeam, user.Squad.RoomType);

                    }
                }
                return false;
            }
            return false;
        }
    }
}
