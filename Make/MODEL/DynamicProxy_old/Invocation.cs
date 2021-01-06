using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.MODEL.DynamicProxy
{
    /// <summary>
    /// 调用器
    /// </summary>
    public class Invocation
    {
        public object[] Parameter { get; set; }
        public Delegate DelegateMethod { get; set; }
        public object Proceed()
        {
            return this.DelegateMethod.DynamicInvoke(Parameter);
        }
    }
}
