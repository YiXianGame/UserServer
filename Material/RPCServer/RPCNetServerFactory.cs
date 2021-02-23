using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Material.RPCServer.TCP_Async_Event;

namespace Material.RPCServer
{
    /// <summary>
    /// 客户端工厂
    /// </summary>
    public class RPCNetServerFactory
    {
        private static Dictionary<Tuple<string, string>, SocketListener> socketservers { get; } = new Dictionary<Tuple<string, string>, SocketListener>();

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="serverIp">远程服务IP</param>
        /// <param name="port">远程服务端口</param>
        /// <returns>客户端</returns>
        public static SocketListener StartServer(string hostname,string port,BaseUserToken.GetInstance createMethod)
        {
            Tuple<string, string> key = new Tuple<string, string>(hostname, port);
            SocketListener socketserver;
            socketservers.TryGetValue(key, out socketserver);
            if (socketserver == null)
            {
                try
                {
                    socketserver = new SocketListener(key.Item1, key.Item2, 1000, 1024,createMethod);
                    for(int i = 0; i < 5; i++)
                    {
                        Thread thread = new Thread(() => socketserver.StartAccept(null));
                        thread.Name = i.ToString();
                        thread.Start();
                    }   
                    socketservers[key] = socketserver;
                }
                catch (SocketException e)
                {
                    Console.WriteLine("发生异常报错,销毁注册");
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);
                    socketserver.Dispose();
                }
            }
            return socketserver;
        }
        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="serverIp">远程服务IP</param>
        /// <param name="port">远程服务端口</param>
        /// <returns>客户端</returns>
        public static SocketListener GetServer(Tuple<string, string> key)
        {
            SocketListener socketserver;
            socketservers.TryGetValue(key, out socketserver);
            return socketserver;
        }
        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="serverIp">远程服务IP</param>
        /// <param name="port">远程服务端口</param>
        /// <returns>客户端</returns>
        public static ConcurrentDictionary<object, BaseUserToken> GetTokens(Tuple<string, string> key)
        {
            SocketListener socketserver;
            socketservers.TryGetValue(key, out socketserver);
            return socketserver.Tokens;
        }
    }
}
