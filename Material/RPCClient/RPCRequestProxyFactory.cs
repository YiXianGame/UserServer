using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Material.RPCClient
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
        public static T Register<T>(string servicename,string hostname, string port,RPCRequestConfig config)
        {
            T service = default(T);
            Tuple<string, string, string> key = new Tuple<string, string, string>(servicename, hostname, port);
            services.TryGetValue(key,out object obj);
            service = (T)obj;
            if(service == null)
            {
                Tuple<string, string> clientkey = new Tuple<string, string>(hostname, port);
                service = RPCRequestProxy.Register<T>(servicename, clientkey, config);
                services[key] = service;
            }
            return service;
        }
        public static void Destory(Tuple<string, string, string> key)
        {
            services.Remove(key, out object value);
        }
    }
}
