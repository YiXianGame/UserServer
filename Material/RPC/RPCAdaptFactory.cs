using Material.TCP_Async_Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Material.RPC
{
    public class RPCAdaptFactory
    { 
        public static ConcurrentDictionary<Tuple<string, string, string>, RPCAdaptProxy> services { get; } = new ConcurrentDictionary<Tuple<string, string, string>, RPCAdaptProxy>();

        public static void Register<R>(string servicename,string hostname, string port,RPCType type,BaseUserToken.GetInstance createMethod) where R:class
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
            Tuple<string, string, string> key = new Tuple<string, string, string>(servicename, hostname, port.ToString());
            services.TryGetValue(key,out service);
            if(service == null)
            {
                try
                {
                    SocketListener socketListener = RPCNetFactory.GetServer(new Tuple<string, string>(hostname, port),createMethod);
                    service = new RPCAdaptProxy();
                    service.Register<R>(type);
                    services[key] = service;
                }
                catch (RPCException err)
                {
                    Console.WriteLine(err.Message + "\n" + err.StackTrace);
                    Destory(servicename, hostname, port);
                }
                catch (Exception err)
                {
                    Console.WriteLine("发生异常报错,销毁注册");
                    Destory(servicename, hostname, port);
                }
            }
        }

        public static void Destory(string servicename, string hostname, string port)
        {
            services.TryRemove(new Tuple<string, string, string>(servicename,hostname,port), out RPCAdaptProxy value);
            RPCNetFactory.Destory(new Tuple<string, string>(hostname, port));
        }
    }
}
