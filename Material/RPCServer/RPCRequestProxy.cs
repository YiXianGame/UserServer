using System;
using System.Reflection;
using System.Text;
using Material.RPCServer.Annotation;
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
                    if (rpcAttribute.Paramters == null)
                    {
                        obj = new string[param_count - 1];
                        for (int i = 1; i < param_count; i++)
                        {
                            try
                            {
                                methodid.Append("-" + type.AbstractName[args[i].GetType()]);
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
                        string[] types_name = rpcAttribute.Paramters.Split('-');
                        if(param_count == types_name.Length)
                        {
                            obj = new string[param_count - 1];
                            for (int i = 1; i < param_count; i++)
                            {
                                if (type.AbstractType.ContainsKey(types_name[i]))
                                {
                                    methodid.Append("-" + type.AbstractName[args[i].GetType()]);
                                    obj[i - 1] = JsonConvert.SerializeObject(args[i]);
                                }
                                else throw new RPCException($"C#对应的{types_name[i]}-{args[i].GetType()}类型参数尚未注册");
                            }
                        }
                        else throw new RPCException($"方法体{targetMethod.Name}中[RPCMethod]与实际参数数量不符,[RPCMethod]:{types_name.Length}个,Method:{param_count}个");
                    }

                }
                ServerRequestModel request = new ServerRequestModel("2.0", servicename, methodid.ToString(), obj);
                if (args[0] != null && (args[0] as BaseUserToken).Socket != null)
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
