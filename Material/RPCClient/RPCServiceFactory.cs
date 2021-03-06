using Material.RPCClient.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Material.RPCClient
{
    public class RPCServiceFactory
    {
        public static Dictionary<Tuple<string, string, string>, RPCService> services { get; } = new Dictionary<Tuple<string, string, string>, RPCService>();
        public static void Register<T>(string servicename, string hostname, string port, RPCServiceConfig config) where T : class
        {
            Register<T>(null, servicename, hostname, port, config);
        }
        public static void Register<T>(T instance,string servicename,string hostname, string port, RPCServiceConfig config) where T:class
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

            if (config.Type is null)
            {
                throw new ArgumentNullException(nameof(config.Type));
            }
            RPCService service = null;
            Tuple<string, string, string> key = new Tuple<string, string, string>(servicename, hostname, port);
            services.TryGetValue(key,out service);
            if(service == null)
            {
                try
                {
                    SocketClient socketClient = RPCNetFactory.GetClient(new Tuple<string, string>(hostname, port));
                    service = new RPCService();
                    service.Register(instance,config);
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
            services.Remove(new Tuple<string, string, string>(servicename,hostname,port), out RPCService value);
        }
        public static bool Get(string servicename, string hostname, string port ,out RPCService proxy)
        {
            return services.TryGetValue(new Tuple<string, string, string>(servicename, hostname, port), out proxy);
        }
    }
}
