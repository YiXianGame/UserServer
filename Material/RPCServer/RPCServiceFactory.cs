﻿using System;
using System.Collections.Concurrent;
using Material.RPCServer.TCP_Async_Event;

namespace Material.RPCServer
{
    public class RPCServiceFactory
    { 
        public static ConcurrentDictionary<Tuple<string, string, string>, RPCService> services { get; } = new ConcurrentDictionary<Tuple<string, string, string>, RPCService>();

        public static void Register<R>(string servicename, string hostname, string port, RPCServiceConfig config) where R : class
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
            services.TryGetValue(key, out RPCService service);
            if (service == null)
            {
                try
                {
                    service = RPCService.Register<R>(config);
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

        public static void Destory(string servicename, string hostname, string port)
        {
            services.TryRemove(new Tuple<string, string, string>(servicename,hostname,port), out RPCService value);
        }

        public static bool Get(string servicename, string hostname, string port, out RPCService proxy)
        {
            return services.TryGetValue(new Tuple<string, string, string>(servicename, hostname, port), out proxy);
        }
    }
}
