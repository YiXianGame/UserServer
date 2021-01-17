using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Make.MODEL.RPC
{
    public class RPCType
    {
        public delegate object ConvertDelegage(object obj);
        public Dictionary<Type, string> TypeToAbstract { get; set; } = new Dictionary<Type, string>();
        public Dictionary<string, RPCType.ConvertDelegage> AbstractToType { get; set; } = new Dictionary<string, ConvertDelegage>();

        public RPCType()
        {

        }
        public void Add<T>(string typeName)
        {
            try
            {
                TypeToAbstract.Add(typeof(T), typeName);
                AbstractToType.Add(typeName, obj => Convert.ChangeType(obj, typeof(T)));
            }
            catch(ArgumentException)
            {
                if (AbstractToType.ContainsKey(typeName) || TypeToAbstract.ContainsKey(typeof(T))) Console.WriteLine($"类型:{typeof(T)}转{typeName}发生异常,存在重复键");
            }
        }
        public void Add<T>(string typeName,ConvertDelegage convert)
        {
            try
            {
                TypeToAbstract.Add(typeof(T), typeName);
                AbstractToType.Add(typeName, convert);
            }
            catch (ArgumentException)
            {
                if (AbstractToType.ContainsKey(typeName) || TypeToAbstract.ContainsKey(typeof(T))) Console.WriteLine($"类型:{typeof(T)}转{typeName}发生异常,存在重复键");
            }
        }
    }
}
