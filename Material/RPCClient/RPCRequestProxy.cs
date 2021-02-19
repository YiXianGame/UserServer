using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Material.RPCClient.TCP_Async_Event;
using Material.RPCServer.TCP_Async_Event;
using Newtonsoft.Json;

namespace Material.RPCClient
{
    public class RPCRequestProxy: DispatchProxy
    {
        private string servicename;
        private Tuple<string,string> key;
        RPCType type;
        public static T Register<T>(string servicename, Tuple<string, string> clientkey, RPCType type)
        {
            if (string.IsNullOrEmpty(servicename))
            {
                throw new ArgumentException("参数为空", nameof(servicename));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            RPCRequestProxy proxy = (RPCRequestProxy)(Create<T, RPCRequestProxy>() as object);
            proxy.key = clientkey ?? throw new ArgumentNullException(nameof(clientkey));
            proxy.servicename = servicename;
            proxy.type = type;
            return (T)(proxy as object);
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                //这里要连接字符串，发现StringBuilder效率高一些.
                StringBuilder methodid = new StringBuilder(targetMethod.Name);
                ParameterInfo[] parameters = targetMethod.GetParameters();
                object[] obj = new object[args.Length + 1];
                for (int i = 0,j = 1; i < args.Length; i++,j++){
                    try
                    {
                        methodid.Append("-" + type.TypeToAbstract[args[i].GetType()]);
                        obj[j] = JsonConvert.SerializeObject(args[i]);
                    }
                    catch (Exception)
                    {
                        throw new RPCException($"C#中的{args[i].GetType()}类型参数尚未注册");
                    }
                }
                ClientRequestModel request = new ClientRequestModel("2.0", servicename, methodid.ToString(), obj);
                if (targetMethod.ReturnType == typeof(void))
                {
                    (RPCNetClientFactory.GetClient(key).SocketArgs.UserToken as Token).SendVoid(request);
                    return null;
                }
                else
                {
                    (RPCNetClientFactory.GetClient(key).SocketArgs.UserToken as Token).Send(request);
                    ClientResponseModel result = request.get();
                    if (type.TypeConvert.TryGetValue(result.Result_Type, out RPCType.ConvertDelegage convert))
                    {
                        convert((string)result.Result);
                    }
                    else throw new RPCException($"C#中的{result.Result_Type}类型转换器尚未注册");
                }
            }
            catch(SocketException e)
            {
                RPCNetClientFactory.GetClient(key).Reconnect();
            }
            return null;
        }
    }
}
        