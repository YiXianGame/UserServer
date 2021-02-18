using Material.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.Model
{
    public class MatchSystem
    {
        #region --字段--
        private Queue<long> solo = new Queue<long>();//单挑匹配队列
        private Tuple<string, string> key;
        private int needUsers = 9999;
        #endregion

        #region --属性--

        public Queue<long> Solo { get => solo; set => solo = value; }
        #endregion

        #region --方法--
        public MatchSystem(Tuple<string, string> key,int needUsers)
        {
            this.key = key;
            this.needUsers = needUsers;
        }
        public void Enter(UserToken userToken)
        {
            lock (solo)
            {
                if (solo.Count > 0)
                {
                    List<UserToken> users = new List<UserToken>();
                    List<long> usersId = new List<long>();
                    users.Add(userToken);
                    StringBuilder roomToken =new StringBuilder();
                    for(int i = 0; i < 1; i++)
                    {
                        if (userToken.GetToken(solo.Dequeue(), out BaseUserToken user) && user.Connected)
                        {
                            users.Add((user as UserToken));
                            usersId.Add((user as UserToken).UserId);
                            roomToken.Append((user as UserToken).UserId);
                        }
                    }
                    foreach (UserToken user in users)
                    {
                        Core.Repository.UserRepository.Update_CardGroups
                        Core.UserRequest.MatchSucess(user,usersId,key.Item1,key.Item2,roomToken.ToString());
                    }
                }
                else solo.Enqueue(userToken.UserId);
            }
        }

        #endregion
    }
}
