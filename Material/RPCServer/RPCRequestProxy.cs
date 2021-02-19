using System;
using System.Reflection;
using System.Text;
using Material.RPCServer.TCP_Async_Event;
using Newtonsoft.Json;

namespace Material.RPCServer
{
    public class RPCRequestProxy : DispatchProxy
    {
        private string servicename;
        RPCType type;

        public static T Register<T>(string servicename,RPCType type)
        {
            RPCRequestProxy proxy = (RPCRequestProxy)(Create<T, RPCRequestProxy>() as object);
            proxy.servicename = servicename;
            proxy.type = type;
            return (T)(proxy as object);
        }


        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            //这里要连接字符串，发现StringBuilder效率高一些.
            StringBuilder methodid = new StringBuilder(targetMethod.Name);
            ParameterInfo[] parameters = targetMethod.GetParameters();
            string[] obj = null;
            if (parameters.Length > 1)
            {
                obj = new string[parameters.Length - 1];
                for (int i = 1; i < parameters.Length; i++)
                {
                    try
                    {
                        methodid.Append("-" + type.TypeToAbstract[parameters[i].ParameterType]);
                        obj[i - 1] = JsonConvert.SerializeObject(args[i]);
                    }
                    catch (Exception)
                    {
                        throw new RPCException($"C#对应的{parameters[i].ParameterType}类型参数尚未注册");
                    }
                }
            }
            ServerRequestModel request = new ServerRequestModel("2.0", servicename, methodid.ToString(), obj);
            if (args[0].GetType() == typeof(BaseUserToken))
            {
                (args[0] as BaseUserToken).Send(request);
            }
            return null;
        }
    }
}
