using Make.MODEL.TCP_Async_Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Make.MODEL.RPC
{
    public class RPCAdaptFactory
    {
        public static ConcurrentDictionary<Tuple<string, string, string>, RPCAdaptProxy> services { get; } = new ConcurrentDictionary<Tuple<string, string, string>, RPCAdaptProxy>();

        public static void Register<T>(string servicename,string hostname, string port) where T:class
        {
            RPCAdaptProxy service = null;
            Tuple<string, string, string> key = new Tuple<string, string, string>(servicename, hostname, port.ToString());
            services.TryGetValue(key,out service);
            if(service == null)
            {
                try
                {
                    SocketListener socketListener = RPCServerFactory.GetServer(new Tuple<string, string>(hostname, port));
                    service = new RPCAdaptProxy();
                    service.Register<T>();
                    services[key] = service;
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
            value.Dispose();
            bool flag = false;
            foreach(Tuple<string,string,string> item in services.Keys)
            {
                if (item.Item2 == hostname && item.Item3 == port) flag = true;
            }
            if (flag == false) RPCServerFactory.Destory(new Tuple<string, string>(hostname, port));
        }
    }
}
