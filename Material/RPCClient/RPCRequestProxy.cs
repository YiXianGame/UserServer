using Material.RPCClient.Annotation;
using Material.RPCClient.TCP_Async_Event;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

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
            RPCRequest rpcAttribute = targetMethod.GetCustomAttribute<RPCRequest>();
            if(rpcAttribute != null)
            {
                try
                {
                    //这里要连接字符串，发现StringBuilder效率高一些.
                    StringBuilder methodid = new StringBuilder(targetMethod.Name);
                    int param_count;
                    if (args != null) param_count = args.Length;
                    else param_count = 0;
                    object[] obj = new object[param_count + 1];
                    if (rpcAttribute.Paramters == null)
                    {
                        for (int i = 0, j = 1; i < param_count; i++, j++)
                        {
                            try
                            {
                                methodid.Append("-" + type.AbstractName[args[i].GetType()]);
                                obj[j] = JsonConvert.SerializeObject(args[i]);
                            }
                            catch (Exception)
                            {
                                throw new RPCException($"C#中的{args[i].GetType()}类型参数尚未注册");
                            }
                        }
                    }
                    else
                    {
                        string[] types_name = rpcAttribute.Paramters;
                        if (types_name.Length == param_count)
                        {
                            for (int i = 0, j = 1; i < param_count; i++, j++)
                            {
                                try
                                {
                                    methodid.Append("-" + types_name[i]);
                                    obj[j] = JsonConvert.SerializeObject(args[i]);
                                }
                                catch (Exception)
                                {
                                    throw new RPCException($"C#中的{args[i].GetType()}类型参数尚未注册");
                                }
                            }
                        }
                        else throw new RPCException($"方法体{targetMethod.Name}中[RPCMethod]与实际参数数量不符,[RPCMethod]:{types_name.Length}个,Method:{param_count}个");
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
                catch (SocketException e)
                {
                    Console.WriteLine("发送请求失败，尝试重连\n" + e.StackTrace);
                    RPCNetClientFactory.GetClient(key).Reconnect();
                }
            }

            return null;
        }
    }
}
        