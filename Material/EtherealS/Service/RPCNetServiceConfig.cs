using System.Reflection;
using Material.EtherealS.Model;

namespace Material.EtherealS.Service
{
    public class RPCNetServiceConfig
    {
        #region --委托--
        public delegate bool InterceptorDelegate(RPCNetService service, MethodInfo method, BaseUserToken token);
        #endregion

        #region --事件--
        public event InterceptorDelegate InterceptorEvent;
        #endregion

        #region --字段--
        private bool tokenEnable = true;
        private RPCType type;
        private bool authoritable = false;
        #endregion

        #region --属性--
        public bool TokenEnable { get => tokenEnable; set => tokenEnable = value; }
        public RPCType Type { get => type; set => type = value; }
        public bool Authoritable { get => authoritable; set => authoritable = value; }
        #endregion

        #region --方法--
        public RPCNetServiceConfig(RPCType type)
        {
            this.type = type;
        }

        public RPCNetServiceConfig(RPCType type, bool tokenEnable)
        {
            this.type = type;
            this.tokenEnable = tokenEnable;
        }
        public bool OnInterceptor(RPCNetService service, MethodInfo method, BaseUserToken token)
        {
            if (InterceptorEvent != null)
            {
                foreach (InterceptorDelegate item in InterceptorEvent.GetInvocationList())
                {
                    if (!item.Invoke(service, method, token)) return false;
                }
                return true;
            }
            else return true;
        }
        #endregion
    }
}
