using System.Reflection;
using Material.EtherealS.Model;
using Material.EtherealS.Service;

namespace Material.EtherealS.Net
{
    public class RPCNetConfig
    {
        #region --委托--
        public delegate bool InterceptorDelegate(RPCNetService service,MethodInfo method,BaseUserToken token);
        #endregion

        #region --事件--
        public event InterceptorDelegate InterceptorEvent;
        #endregion

        #region --字段--
        private int numConnections = 1024;
        private int bufferSize = 1024;
        private int numChannels = 5;
        private bool autoManageTokens = true;
        private BaseUserToken.GetInstance createMethod;
        
        #endregion

        #region --属性--
        public bool AutoManageTokens { get => autoManageTokens; set => autoManageTokens = value; }
        public int NumConnections { get => numConnections; set => numConnections = value; }
        public int BufferSize { get => bufferSize; set => bufferSize = value; }
        public int NumChannels { get => numChannels; set => numChannels = value; }
        public BaseUserToken.GetInstance CreateMethod { get => createMethod; set => createMethod = value; }

        #endregion

        #region --方法--
        public RPCNetConfig(BaseUserToken.GetInstance createMethod)
        {
            this.createMethod = createMethod;
        }
        public bool OnInterceptor(RPCNetService service,MethodInfo method,BaseUserToken token)
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
