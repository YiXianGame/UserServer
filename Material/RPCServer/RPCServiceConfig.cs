using Material.RPCServer.Extension;
using Material.RPCServer.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Material.RPCServer
{
    public class RPCServiceConfig
    {
        #region --委托--
        public delegate bool InterceptorDelegate(RPCService service, MethodInfo method, BaseUserToken token);
        #endregion

        #region --事件--
        public event InterceptorDelegate InterceptorEvent;
        #endregion

        #region --字段--
        private bool tokenEnable;
        private RPCType type;
        private bool authoritable = false;
        #endregion

        #region --属性--
        public bool TokenEnable { get => tokenEnable; set => tokenEnable = value; }
        public RPCType Type { get => type; set => type = value; }
        public bool Authoritable { get => authoritable; set => authoritable = value; }
        #endregion

        #region --方法--
        public RPCServiceConfig(RPCType type)
        {
            this.type = type;
        }

        public RPCServiceConfig(RPCType type, bool tokenEnable)
        {
            this.type = type;
            this.tokenEnable = tokenEnable;
        }
        public bool OnInterceptor(RPCService service, MethodInfo method, BaseUserToken token)
        {
            foreach (InterceptorDelegate item in InterceptorEvent.GetInvocationList())
            {
                if (!item.Invoke(service, method, token)) return false;
            }
            return true;
        }
        #endregion
    }
}
