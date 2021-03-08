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
    }
}
