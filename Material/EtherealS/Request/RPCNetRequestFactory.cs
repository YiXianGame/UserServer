using System;
using System.Collections.Generic;

namespace Material.EtherealS.Request
{
    public class RPCNetRequestFactory
    {
        private static Dictionary<Tuple<string, string, string>, RPCNetRequestProxy> requests { get; } = new Dictionary<Tuple<string, string, string>, RPCNetRequestProxy>();
        /// <summary>
        /// 获取RPC代理
        /// </summary>
        /// <param name="servicename">服务名</param>
        /// <param name="serverIp">远程服务IP</param>
        /// <param name="port">远程服务端口</param>
        /// <returns>客户端</returns>
        public static R Register<R>(string servicename,string hostname, string port,RPCNetRequestConfig config)
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

            if (config.Type is null)
            {
                Console.WriteLine($"{servicename}-{hostname}-{port} Load Fail!");
                throw new ArgumentNullException(nameof(config.Type));
            }
            Tuple<string, string, string> key = new Tuple<string, string, string>(servicename, hostname, port);
            requests.TryGetValue(key,out RPCNetRequestProxy request);
            if(request == null)
            {
                request = RPCNetRequestProxy.Register<R>(servicename,config);
                requests[key] = request;
                Console.WriteLine($"{servicename}-{hostname}-{port} Load Success!");
            }
            return (R)(request as object);
        }
        public static void Destory(Tuple<string, string, string> key)
        {
            requests.Remove(key, out RPCNetRequestProxy value);
        }
    }
}
