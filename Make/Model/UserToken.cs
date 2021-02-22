using Material.Entity;
using Material.Interface;
using Material.RPCServer.TCP_Async_Event;
using System.Collections.Generic;

namespace Make.Model
{
    public class UserToken : BaseUserToken,IMatchSystemItem
    {
        #region --字段--
        private long userId = -1;
        private int rank = 0;//队伍分数
        private long startMatchTime = 0;//开始匹配时间
        private int averageRank = 0;
        private int count = 0;
        #endregion

        #region --属性--
        public long UserId { get => userId; set => userId = value; }
        public long StartMatchTime { get => startMatchTime; set => startMatchTime = value; }
        public int Count { get => count; set => count = value; }
        public int Rank { get => rank; set => rank = value; }
        public int AverageRank { get => averageRank; set => averageRank = value; }
        #endregion

        #region --重写方法--
        public override void Init()
        {
            UserId = -1;
        }
        public override object GetKey()
        {
            return UserId;
        }
        public bool GetToken(long id, out UserToken value)
        {
            return GetToken(id, out value);
        }

        public void Add(UserToken team)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(UserToken team)
        {
            throw new System.NotImplementedException();
        }
        #endregion

    }
}
