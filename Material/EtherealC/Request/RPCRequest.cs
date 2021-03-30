using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Material.EtherealC.Model;
using Material.EtherealC.Net;
using Material.EtherealC.Net.AsyncClient;
using Newtonsoft.Json;

namespace Material.EtherealC.Request
{
    public class RPCRequest : DispatchProxy
    {
        private string servicename;
        private Tuple<string,string> key;
        private RPCRequestConfig config;
        private int paramStart;
        public static T Register<T>(string servicename, Tuple<string, string> clientkey, RPCRequestConfig config)
        {
            if (string.IsNullOrEmpty(servicename))
            {
                throw new ArgumentException("参数为空", nameof(servicename));
            }

            if (config.Type is null)
            {
                throw new ArgumentNullException(nameof(config.Type));
            }
            RPCRequest proxy = (RPCRequest)(Create<T, RPCRequest>() as object);
            proxy.key = clientkey ?? throw new ArgumentNullException(nameof(clientkey));
            proxy.servicename = servicename; 
            proxy.config = config;
            if (config.TokenEnable) proxy.paramStart = 1;
            else proxy.paramStart = 0;
            return (T)(proxy as object);
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            Attribute.RPCRequest rpcAttribute = targetMethod.GetCustomAttribute<Attribute.RPCRequest>();
            if(rpcAttribute != null)
            {
                try
                {
                    //这里要连接字符串，发现StringBuilder效率高一些.
                    StringBuilder methodid = new StringBuilder(targetMethod.Name);
                    int param_count;
                    if (args != null) param_count = args.Length;
                    else param_count = 0;
                    object[] obj = new object[param_count + paramStart];
                    if (rpcAttribute.Paramters == null)
                    {
                        ParameterInfo[] parameters = targetMethod.GetParameters();
                        for (int i = 0, j = paramStart; i < param_count; i++, j++)
                        {
                            try
                            {
                                methodid.Append("-" + config.Type.AbstractName[parameters[i].ParameterType]);
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
                            for (int i = 0, j = paramStart; i < param_count; i++, j++)
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
                        (RPCNetFactory.GetClient(key).SocketArgs.UserToken as Token).SendVoid(request);
                        return null;
                    }
                    else
                    {
                        (RPCNetFactory.GetClient(key).SocketArgs.UserToken as Token).Send(request);
                        int timeout = config.Timeout;
                        if (rpcAttribute.Timeout != -1) timeout = rpcAttribute.Timeout;
                        ClientResponseModel result = request.Get(timeout);
                        if(result != null)
                        {
                            if (result.Error != null)
                            {
                                if (result.Error.Code == 0)
                                {
                                    throw new RPCException(RPCException.ErrorCode.Intercepted, $"ErrorCode:{result.Error.Code} Message:{result.Error.Message} Data:{result.Error.Data}");
                                }
                            }
                            else if (config.Type.TypeConvert.TryGetValue(result.ResultType, out RPCType.ConvertDelegage convert))
                            {
                                return convert((string)result.Result);
                            }
                            else throw new RPCException($"C#中的{result.ResultType}类型转换器尚未注册");
                        }
                        return null;
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("发送请求失败，尝试重连\n" + e.StackTrace);
                    RPCNetFactory.GetClient(key).Reconnect();
                }
            }
            return null;
        }
    }
}
        