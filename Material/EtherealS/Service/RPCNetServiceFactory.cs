using System;
using System.Collections.Concurrent;
using Material.EtherealS.Model;

namespace Material.EtherealS.Service
{
    public class RPCServiceFactory
    { 
        public static ConcurrentDictionary<Tuple<string, string, string>, RPCNetService> services { get; } = new ConcurrentDictionary<Tuple<string, string, string>, RPCNetService>();

        public static void Register(object instance,string servicename, string hostname, string port, RPCNetServiceConfig config)
        {
            Console.WriteLine($"{servicename}-{hostname}-{port} Loading...");
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
            Tuple<string, string, string> key = new Tuple<string, string, string>(servicename, hostname, port.ToString());
            services.TryGetValue(key, out RPCNetService service);
            if (service == null)
            {
                try
                {
                    service = new RPCNetService();
                    service.Register(instance,config);
                    services[key] = service;
                    Console.WriteLine($"{servicename}-{hostname}-{port} Load Success!");
                }
                catch (RPCException e)
                {
                    Console.WriteLine($"{servicename}-{hostname}-{port} Load Fail!");
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);
                    Destory(servicename, hostname, port);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{servicename}-{hostname}-{port} Load Fail!");
                    Console.WriteLine("发生异常报错,销毁注册");
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);
                    Destory(servicename, hostname, port);
                }
            }
        }

        public static void Register<T>(string servicename, string hostname, string port, RPCNetServiceConfig config) where T : new()
        {
            Register(new T(), servicename, hostname, port, config);
        }

        public static void Destory(string servicename, string hostname, string port)
        {
            services.TryRemove(new Tuple<string, string, string>(servicename,hostname,port), out RPCNetService value);
        }

        public static bool Get(string servicename, string hostname, string port, out RPCNetService proxy)
        {
            return services.TryGetValue(new Tuple<string, string, string>(servicename, hostname, port), out proxy);
        }
    }
}
