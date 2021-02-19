using Make.Model.GameModel;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Material.RPCServer.TCP_Async_Event;
using System.Collections.Concurrent;

namespace Make.Model
{
    public class UserToken : BaseUserToken
    {
        #region --字段--
        private long userId=-1;

        #endregion

        #region --属性--
        public long UserId { get => userId; set => userId = value; }
        
        #endregion

        #region --重写方法--
        public override void Init()
        {
            userId = -1;
        }
        public override object GetKey()
        {
            return userId;
        }
        public bool GetToken(long id,out UserToken value)
        {
            return GetToken(id, out value);
        }
        #endregion

    }
}
