using Material.RPCServer.Annotation;
using Material.RPCServer.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Material.RPCServer
{
    public class RPCAdaptProxy
    {
        //原作者的思想是Type调用Invoke，这里是在注册的时候就预存方法，1e6情况下调用速度的话是快了4-5倍左右，比正常调用慢10倍
        //猜测类似C++函数指针可能会更快,C#.NET理念下函数指针只能用委托替代，但委托自由度不高.
        //string连接的时候使用引用要比tuple慢很多
        private Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        private RPCType type;
        private object instance;
        public Dictionary<string, MethodInfo> Methods { get => methods; set => methods = value; }
        public RPCType Type { get => type; set => type = value; }
        public object Instance { get => instance; set => instance = value; }

        public void Register<T>(RPCType type)
        {
            this.type = type;
            StringBuilder methodid = new StringBuilder();
            foreach (MethodInfo method in typeof(T).GetMethods())
            {
                RPCAdapt rpcAttribute = method.GetCustomAttribute<RPCAdapt>();
                if (rpcAttribute != null)
                {
                    if (!method.IsAbstract)
                    {
                        methodid.Append(method.Name);
                        ParameterInfo[] parameters = method.GetParameters();
                        if (rpcAttribute.Paramters == null)
                        {
                            for (int i = 1; i < parameters.Length; i++)
                            {
                                if(i == 1 && (parameters[i].ParameterType.IsAssignableFrom(typeof(BaseUserToken))))
                                {
                                    throw new RPCException($"{method.Name}方法中的首参数并非继承于BaseUserToken!");
                                }

                                try
                                {
                                    methodid.Append("-" + type.AbstractName[parameters[i].ParameterType]);
                                }
                                catch (Exception)
                                {
                                    throw new RPCException($"{method.Name}方法中的{parameters[i].ParameterType}类型参数尚未注册");
                                }
                            }
                        }
                        else
                        {
                            string[] types_name = rpcAttribute.Paramters.Split('-');
                            if(parameters.Length == types_name.Length)
                            {
                                //跳过第一个参数Token，本来打算让客户端加上这个参数，但是分析后觉得代码不够友好，还节省资源.
                                for (int i = 1; i < parameters.Length; i++)
                                {
                                    if(type.AbstractType.ContainsKey(types_name[i]))
                                    {
                                        methodid.Append("-").Append(types_name[i]);
                                    }
                                    else throw new RPCException($"C#对应的{types_name[i]}类型参数尚未注册"); 
                                }
                            }
                            else throw new RPCException($"方法体{method.Name}中[RPCMethod]与实际参数数量不符,[RPCMethod]:{types_name.Length}个,Method:{parameters.Length}个");
                        }
                        Methods.TryAdd(methodid.ToString(), method);
                        methodid.Length = 0;
                    }
                }
            }
        }
        public void Register<T>(T instance,RPCType type)
        {
            this.instance = instance;
            Register<T>(type);
        }
        public void ConvertParams(string methodId,object[] parameters)
        {
            String[] param_id = methodId.Split('-');
            if (param_id.Length > 1) {
                for (int i = 1, j = 1; i < param_id.Length && j < parameters.Length; i++,j++)
                {
                    if (Type.TypeConvert.TryGetValue(param_id[i], out RPCType.ConvertDelegage convert))
                    {
                        parameters[j] = convert((string)parameters[j]);
                    }
                    else throw new RPCException($"RPC中的{param_id[i]}类型转换器在TypeConvert字典中尚未被注册");
                }
            }
        }
    }
}
