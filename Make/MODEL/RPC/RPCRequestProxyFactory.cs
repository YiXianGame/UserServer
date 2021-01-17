using Make.MODEL.TCP_Async_Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Make.MODEL.RPC
{
    public class RPCRequestProxyFactory
    {
        private static Dictionary<Tuple<string, string, string>, object> services { get; } = new Dictionary<Tuple<string, string, string>, object>();
        /// <summary>
        /// 获取RPC代理
        /// </summary>
        /// <param name="servicename">服务名</param>
        /// <param name="serverIp">远程服务IP</param>
        /// <param name="port">远程服务端口</param>
        /// <returns>客户端</returns>
        public static T Register<T>(string servicename,string hostname, string port,RPCType type)
        {
            if (string.IsNullOrEmpty(servicename))
            {
                throw new ArgumentException("参数为空", nameof(servicename));
            }

            if (string.IsNullOrEmpty(hostname))
            {
                throw new ArgumentException("参数为空", nameof(hostname));
            }

            if (string.IsNullOrEmpty(port))
            {
                throw new ArgumentException("参数为空", nameof(port));
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            T service = default(T);
            Tuple<string, string, string> key = new Tuple<string, string, string>(servicename, hostname, port);
            services.TryGetValue(key,out object obj);
            service = (T)obj;
            if(service == null)
            {
                Tuple<string, string> serverkey = new Tuple<string, string>(hostname, port);
                RPCServerFactory.GetServer(serverkey);
                service = RPCRequestProxy.Register<T>(servicename, new Tuple<string, string>(hostname, port),type.TypeToAbstract);
                services[key] = service;
            }
            return service;
        }
        public static void Destory(Tuple<string, string, string> key)
        {
            services.Remove(key, out object value);
            RPCServerFactory.Destory(new Tuple<string, string>(key.Item2, key.Item3));
        }
    }
}
