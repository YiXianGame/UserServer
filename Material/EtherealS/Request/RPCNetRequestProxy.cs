using System;
using System.Reflection;
using System.Text;
using Material.EtherealS.Annotation;
using Material.EtherealS.Model;
using Newtonsoft.Json;

namespace Material.EtherealS.Request
{
    public class RPCNetRequestProxy : DispatchProxy
    {
        private string servicename;
        RPCNetRequestConfig config;
        public static RPCNetRequestProxy Register<T>(string servicename, RPCNetRequestConfig config)
        {
            RPCNetRequestProxy proxy = Create<T, RPCNetRequestProxy>() as RPCNetRequestProxy;
            proxy.servicename = servicename;
            proxy.config = config;
            return proxy;
        }
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            RPCRequest rpcAttribute = targetMethod.GetCustomAttribute<RPCRequest>();
            if (rpcAttribute != null)
            {
                //这里要连接字符串，发现StringBuilder效率高一些.
                StringBuilder methodid = new StringBuilder(targetMethod.Name);
                string[] obj = null;
                int param_count;
                if (args != null) param_count = args.Length;
                else param_count = 0;
                if (param_count > 1)
                {
                    obj = new string[param_count - 1];
                    if (rpcAttribute.Paramters == null)
                    {
                        ParameterInfo[] parameters = targetMethod.GetParameters();
                        for (int i = 1; i < param_count; i++)
                        {
                            try
                            {
                                methodid.Append("-" + config.Type.AbstractName[parameters[i].ParameterType]);
                                obj[i - 1] = JsonConvert.SerializeObject(args[i]);
                            }
                            catch (Exception)
                            {
                                throw new RPCException($"C#对应的{args[i].GetType()}类型参数尚未注册");
                            }
                        }
                    }
                    else
                    {
                        string[] types_name = rpcAttribute.Paramters;
                        if(param_count == types_name.Length)
                        {
                            for (int i = 1; i < param_count; i++)
                            {
                                if (config.Type.AbstractType.ContainsKey(types_name[i]))
                                {
                                    methodid.Append("-" + types_name[i]);
                                    obj[i - 1] = JsonConvert.SerializeObject(args[i]);
                                }
                                else throw new RPCException($"C#对应的{types_name[i]}-{args[i].GetType()}类型参数尚未注册");
                            }
                        }
                        else throw new RPCException($"方法体{targetMethod.Name}中[RPCMethod]与实际参数数量不符,[RPCMethod]:{types_name.Length}个,Method:{param_count}个");
                    }

                }
                ServerRequestModel request = new ServerRequestModel("2.0", servicename, methodid.ToString(), obj);
                if (args[0] != null && (args[0] as BaseUserToken).Socket != null && (args[0] as BaseUserToken).Socket.Connected)
                {
                    (args[0] as BaseUserToken).Send(request);
                    return null;
                }
                else return null;// throw new RPCException($"方法体:{methodid} 执行时，缺少首参数BaseUserToken，请检查是否传参错误！");
            }   
            else return targetMethod.Invoke(this, args);
        }
    }
}
