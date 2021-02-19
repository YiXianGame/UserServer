using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Material.RPCClient
{
    public class RPCAdaptProxy
    {
        //原作者的思想是Type调用Invoke，这里是在注册的时候就预存方法，1e6情况下调用速度的话是快了4-5倍左右，比正常调用慢10倍
        //猜测类似C++函数指针可能会更快,C#.NET理念下函数指针只能用委托替代，但委托自由度不高.
        //string连接的时候使用引用要比tuple慢很多
        private Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        private RPCType type;

        public Dictionary<string, MethodInfo> Methods { get => methods;  }

        public void Register<T>(RPCType type)
        {
            StringBuilder methodid = new StringBuilder();
            this.type = type;
            foreach (MethodInfo method in typeof(T).GetMethods())
            {
                if(method.IsPublic && !method.IsAbstract && method.IsStatic)
                {
                    methodid.Append(method.Name);
                    ParameterInfo[] parameters = method.GetParameters();
                    foreach(ParameterInfo param in parameters)
                    {
                        try
                        {
                            methodid.Append("-" + type.TypeToAbstract[param.ParameterType]);
                        }
                        catch (Exception)
                        {
                            throw new RPCException($"C#中的{param.ParameterType}类型参数尚未注册");
                        }
                    }
                    Methods.TryAdd(methodid.ToString(), method);
                    methodid.Length = 0;
                }
            }
        }
        public void ConvertParams(string methodId, object[] parameters)
        {
            String[] param_id = methodId.Split('-');
            if (param_id.Length > 1)
            {
                for (int i = 1,j=0; i < param_id.Length; i++,j++)
                {
                    if (type.TypeConvert.TryGetValue(param_id[i], out RPCType.ConvertDelegage convert))
                    {
                        parameters[j] = convert((string)parameters[j]);
                    }
                    else throw new RPCException($"RPC中的{param_id[i]}类型转换器在TypeConvert字典中尚未被注册");
                }
            }
        }
    }
}
