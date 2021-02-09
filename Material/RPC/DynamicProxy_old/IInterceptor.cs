using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.MODEL.DynamicProxy
{
    /// <summary>
    /// 拦截器
    /// </summary>
    public interface IInterceptor
    {
        object Intercept(Invocation invocation);
    }
}
