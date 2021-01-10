using Make.MODEL.TCP_Async_Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Make.MODEL.RPC
{
    public class RPCRequestProxy : DispatchProxy
    {
        public Random random = new Random();
        private string servicename;
        private Tuple<string, string> key;
        public static T Create<T>(string servicename, Tuple<string, string> clientkey)
        {
            RPCRequestProxy proxy = (RPCRequestProxy)(Create<T, RPCRequestProxy>() as object);
            proxy.key = clientkey;
            proxy.servicename = servicename;
            return (T)(proxy as object);
        }


        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            //这里要连接字符串，发现StringBuilder效率高一些.
            StringBuilder methodid = new StringBuilder(targetMethod.Name);
            ParameterInfo[] parameters = targetMethod.GetParameters();
            for (int i = 1; i < parameters.Length; i++)
            {
                methodid.Append("-");
                methodid.Append(parameters[i].ParameterType.Name);
            }
            object[] obj;
            if (args.Length > 1)
            {
                //这里装盒，空下一个空的object位置，服务器到时候会用来装Token
                obj = new Object[args.Length - 1];
                for(int i = 1; i < args.Length; i++)
                {
                    obj[i - 1] = args[i];
                }
            }
            else
            {
                obj = null;
            }
            ServerRequestModel request = new ServerRequestModel("2.0", servicename, methodid.ToString(), obj);
            if (args[0].GetType() == typeof(Token))
            {
                (args[0] as Token).Send(request);
            }
            return null;
        }
    }
}
