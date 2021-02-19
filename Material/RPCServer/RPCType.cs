using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Material.RPCServer
{
    public class RPCType
    {
        public delegate object ConvertDelegage(string obj);
        public Dictionary<Type, string> TypeToAbstract { get; set; } = new Dictionary<Type, string>();
        public Dictionary<string, ConvertDelegage> TypeConvert { get; set; } = new Dictionary<string, ConvertDelegage>();

        public RPCType()
        {

        }
        public void Add<T>(string typeName)
        {
            try
            {
                TypeToAbstract.Add(typeof(T), typeName);
                TypeConvert.Add(typeName, obj=>JsonConvert.DeserializeObject<T>(obj));
            }
            catch (Exception)
            {
                if (TypeConvert.ContainsKey(typeName) || TypeToAbstract.ContainsKey(typeof(T))) Console.WriteLine($"注册类型:{typeof(T)}转{typeName}发生异常");
            }
        }
        public void Add<T>(string typeName, ConvertDelegage convert)
        {
            try
            {
                TypeToAbstract.Add(typeof(T), typeName);
                TypeConvert.Add(typeName, convert);
            }
            catch (Exception)
            {
                if ( TypeConvert.ContainsKey(typeName) || TypeToAbstract.ContainsKey(typeof(T))) Console.WriteLine($"注册类型:{typeof(T)}转{typeName}发生异常");
            }
        }
    }
}
