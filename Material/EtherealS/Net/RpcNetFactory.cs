using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Material.EtherealS.Model;
using Material.EtherealS.Net.AsyncServer;

namespace Material.EtherealS.Net
{
    /// <summary>
    /// 客户端工厂
    /// </summary>
    public class RPCNetFactory
    {
        private static Dictionary<Tuple<string, string>, SocketListener> socketservers { get; } = new Dictionary<Tuple<string, string>, SocketListener>();

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="serverIp">远程服务IP</param>
        /// <param name="port">远程服务端口</param>
        /// <returns>客户端</returns>
        public static SocketListener StartServer(string host, string port, RPCNetConfig config)
        {
            Tuple<string, string> key = new Tuple<string, string>(host, port);
            SocketListener socketserver;
            socketservers.TryGetValue(key, out socketserver);
            if (socketserver == null)
            {
                try
                {
                    socketserver = new SocketListener(host,port,config);
                    for (int i = 0; i < config.NumChannels; i++)
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
