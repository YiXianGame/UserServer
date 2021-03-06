using Material.RPCServer.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Material.RPCServer
{
    public class RPCNetConfig
    {
        #region --委托--
        public delegate bool InterceptorDelegate(RPCService service,MethodInfo method,BaseUserToken token);
        #endregion

        #region --事件--
        public event InterceptorDelegate InterceptorEvent;
        #endregion

        #region --字段--
        private string host;
        private string port;
        private int numConnections = 1024;
        private int bufferSize = 1024;
        private int numChannels = 5;
        private bool autoManageTokens = true;
        private BaseUserToken.GetInstance createMethod;
        
        #endregion

        #region --属性--
        public string Host { get => host; set => host = value; }
        public string Port { get => port; set => port = value; }
        public bool AutoManageTokens { get => autoManageTokens; set => autoManageTokens = value; }
        public int NumConnections { get => numConnections; set => numConnections = value; }
        public int BufferSize { get => bufferSize; set => bufferSize = value; }
        public int NumChannels { get => numChannels; set => numChannels = value; }
        public BaseUserToken.GetInstance CreateMethod { get => createMethod; set => createMethod = value; }

        #endregion

        #region --方法--
        public RPCNetConfig(string host, string port, BaseUserToken.GetInstance createMethod)
        {
            this.host = host;
            this.port = port;
            this.createMethod = createMethod;
        }
        public bool OnInterceptor(RPCService service,MethodInfo method,BaseUserToken token)
        {
            foreach(InterceptorDelegate item in InterceptorEvent.GetInvocationList())
            {
                if (!item.Invoke(service, method, token)) return false;
            }
            return true;
        }
        #endregion
    }
}
