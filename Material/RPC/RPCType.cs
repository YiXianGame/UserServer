using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Material.RPC
{
    public class RPCType
    {
        public delegate object ConvertDelegage(object obj);
        public Dictionary<Type, string> TypeToAbstract { get; set; } = new Dictionary<Type, string>();
        public Dictionary<string,Type> AbstractToType { get; set; } = new Dictionary<string,Type>();
        public Dictionary<string, ConvertDelegage> TypeConvert { get; set; } = new Dictionary<string, ConvertDelegage>();

        public RPCType()
        {

        }
        public void Add<T>(string typeName)
        {
            try
            {
                TypeToAbstract.Add(typeof(T), typeName);
                AbstractToType.Add(typeName, typeof(T));
                TypeConvert.Add(typeName, obj => ((JObject)obj).ToObject<T>());
            }
            catch (Exception)
            {
                if (AbstractToType.ContainsKey(typeName) || TypeConvert.ContainsKey(typeName) || TypeToAbstract.ContainsKey(typeof(T))) Console.WriteLine($"注册类型:{typeof(T)}转{typeName}发生异常");
            }
        }
        public void Add<T>(string typeName, ConvertDelegage convert)
        {
            try
            {
                TypeToAbstract.Add(typeof(T), typeName);
                AbstractToType.Add(typeName, typeof(T));
                TypeConvert.Add(typeName, convert);
            }
            catch (Exception)
            {
                if (AbstractToType.ContainsKey(typeName) || TypeConvert.ContainsKey(typeName) || TypeToAbstract.ContainsKey(typeof(T))) Console.WriteLine($"注册类型:{typeof(T)}转{typeName}发生异常");
            }
        }
    }
}
