using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Make.MODEL.RPC
{
    public class RPCAdaptProxy : IDisposable
    {
        //原作者的思想是Type调用Invoke，这里是在注册的时候就预存方法，1e6情况下调用速度的话是快了4-5倍左右，比正常调用慢10倍
        //猜测类似C++函数指针可能会更快,C#.NET理念下函数指针只能用委托替代，但委托自由度不高.
        //string连接的时候使用引用要比tuple慢很多
        public ConcurrentDictionary<string, MethodInfo> methods = new ConcurrentDictionary<string, MethodInfo>();

        public void Register<T>()
        {
            StringBuilder sb = new StringBuilder();
            foreach (MethodInfo method in typeof(T).GetMethods())
            {
                if(method.IsPublic && !method.IsAbstract && method.IsStatic)
                {
                    sb.Append(method.Name);
                    ParameterInfo[] parameters = method.GetParameters();
                    //跳过第一个参数Token，本来打算让客户端加上这个参数，但是分析后觉得不加最好，还节省资源.
                    for (int i = 1; i < parameters.Length; i++)
                    {
                        sb.Append("-");
                        sb.Append(parameters[i].ParameterType.Name);
                    }
                    methods.TryAdd(sb.ToString(), method);
                    sb.Length = 0;
                }
            }
        }

        #region IDisposable Members
        bool isDipose = false;
        ~RPCAdaptProxy()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (isDipose) return;
            if (disposing)
            {
                methods.Clear();
                methods = null;
            }
            //处理非托管资源
            
            isDipose = true;
        }
        #endregion
    }
}
