using Material.Entity;
using Material.Entity.Game;
using Material.Entity.Match;
using Material.RPCServer.Annotation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Make.RPCServer.Adapt
{
    public class ReadyAdapt
    {

        [RPCAdapt]
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
        [RPCAdapt]
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
        [RPCAdapt]
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
        [RPCAdapt]
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
        [RPCAdapt]
        public void ConfirmCardGroup(User user)
        {
            if (!user.Confirm && user.Squad != null && user.Team != null && user.TeamGroup != null)
            {
                user.Confirm = true;
                bool flag = true;
                foreach (Team team in user.TeamGroup.Items)
                {
                    foreach (Squad squad in team.Items)
                    {
                        foreach (User player in squad.Items)
                        {
                            if (!player.Confirm) flag = false;
                        }
                    }
                }
                if(flag)
                {

                }
            }
        }
    }
}
