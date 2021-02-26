using Material.RPCClient.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Material.RPCClient
{
    public class RPCAdaptFactory
    {
        public static Dictionary<Tuple<string, string, string>, RPCAdaptProxy> services { get; } = new Dictionary<Tuple<string, string, string>, RPCAdaptProxy>();
        public static void Register<T>(string servicename, string hostname, string port, RPCType type) where T : class
        {
            Register<T>(null, servicename, hostname, port, type);
        }
        public static void Register<T>(T instance,string servicename,string hostname, string port, RPCType type) where T:class
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
            RPCAdaptProxy service = null;
            Tuple<string, string, string> key = new Tuple<string, string, string>(servicename, hostname, port);
            services.TryGetValue(key,out service);
            if(service == null)
            {
                try
                {
                    SocketClient socketClient = RPCNetClientFactory.GetClient(new Tuple<string, string>(hostname, port));
                    service = new RPCAdaptProxy();
                    service.Register(instance,type);
                    services[key] = service;
                }
                catch (SocketException e)
                {
                    Console.WriteLine("发生异常报错,销毁注册\n" + e.StackTrace);
                    Destory(servicename, hostname, port);
                }
            }
        }

        public static void Destory(string servicename, string hostname, string port)
        {
            services.Remove(new Tuple<string, string, string>(servicename,hostname,port), out RPCAdaptProxy value);
        }
    }
}
