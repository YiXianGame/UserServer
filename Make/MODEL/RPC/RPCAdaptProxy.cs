using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Make.MODEL.RPC;

namespace Make.MODEL.RPC
{
    public class RPCAdaptProxy
    {
        //原作者的思想是Type调用Invoke，这里是在注册的时候就预存方法，1e6情况下调用速度的话是快了4-5倍左右，比正常调用慢10倍
        //猜测类似C++函数指针可能会更快,C#.NET理念下函数指针只能用委托替代，但委托自由度不高.
        //string连接的时候使用引用要比tuple慢很多
        private Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        private Dictionary<String, RPCType.ConvertDelegage> abstractToType;

        public Dictionary<string, MethodInfo> Methods { get => methods; set => methods = value; }

        public void Register<T>(RPCType type)
        {
            this.abstractToType = type.AbstractToType;
            StringBuilder methodid = new StringBuilder();
            foreach (MethodInfo method in typeof(T).GetMethods())
            {
                if(method.IsPublic && !method.IsAbstract && method.IsStatic)
                {
                    methodid.Append(method.Name);
                    ParameterInfo[] parameters = method.GetParameters();
                    //跳过第一个参数Token，本来打算让客户端加上这个参数，但是分析后觉得不加最好，还节省资源.
                    for (int i = 1; i < parameters.Length; i++)
                    {
                        try
                        {
                            methodid.Append("-" + type.TypeToAbstract[parameters[i].ParameterType]);
                        }
                        catch (Exception)
                        {
                            throw new RPCException($"C#中的{type}类型参数尚未注册");
                        }
                    }
                    Methods.TryAdd(methodid.ToString(), method);
                    methodid.Length = 0;
                }
            }
        }
        public void ConvertParams(string methodId,Object[] parameters)
        {
            String[] param_id = methodId.Split('-') ;
            for(int i = 1; i < param_id.Length; i++)
            {
                if (abstractToType.TryGetValue(param_id[i], out RPCType.ConvertDelegage convert))
                {
                    parameters[i - 1] = convert(parameters[i - 1]);
                }
                else throw new RPCException($"RPC中的{param_id[i]}类型参数尚未被注册");
            }
        }
    }
}
