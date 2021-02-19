using System;
using System.Collections.Generic;
using Material.RPCServer.TCP_Async_Event;

namespace Material.RPCServer
{
    public class RPCRequestProxyFactory
    {
        private static Dictionary<Tuple<string, string, string>, object> requests { get; } = new Dictionary<Tuple<string, string, string>, object>();
        /// <summary>
        /// 获取RPC代理
        /// </summary>
        /// <param name="servicename">服务名</param>
        /// <param name="serverIp">远程服务IP</param>
        /// <param name="port">远程服务端口</param>
        /// <returns>客户端</returns>
        public static R Register<R>(string servicename,string hostname, string port,RPCType type)
        {
            if (string.IsNullOrEmpty(servicename))
            {
                Console.WriteLine($"{servicename}-{hostname}-{port} Load Fail!");
                throw new ArgumentException("参数为空", nameof(servicename));
            }

            if (string.IsNullOrEmpty(hostname))
            {
                Console.WriteLine($"{servicename}-{hostname}-{port} Load Fail!");
                throw new ArgumentException("参数为空", nameof(hostname));
            }

            if (string.IsNullOrEmpty(port))
            {
                Console.WriteLine($"{servicename}-{hostname}-{port} Load Fail!");
                throw new ArgumentException("参数为空", nameof(port));
            }

            if (type is null)
            {
                Console.WriteLine($"{servicename}-{hostname}-{port} Load Fail!");
                throw new ArgumentNullException(nameof(type));
            }

            R request = default(R);
            Tuple<string, string, string> key = new Tuple<string, string, string>(servicename, hostname, port);
            requests.TryGetValue(key,out object obj);
            request = (R)obj;
            if(request == null)
            {
                request = RPCRequestProxy.Register<R>(servicename,type);
                requests[key] = request;
                Console.WriteLine($"{servicename}-{hostname}-{port} Load Sucess!");
            }
            return request;
        }
        public static void Destory(Tuple<string, string, string> key)
        {
            requests.Remove(key, out object value);
        }
    }
}
