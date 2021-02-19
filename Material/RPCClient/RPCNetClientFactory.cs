using Material.RPCClient.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Material.RPCClient
{
    /// <summary>
    /// 客户端工厂
    /// </summary>
    public class RPCNetClientFactory
    {
        private static Dictionary<Tuple<string, string>, SocketClient> socketclients { get; } = new Dictionary<Tuple<string, string>, SocketClient>();
        /// <summary>
        /// 启动客户端
        /// </summary>
        /// <param name="serverIp">远程服务IP</param>
        /// <param name="port">远程服务端口</param>
        /// <returns>客户端</returns>
        public static SocketClient StartClient(string hostname,string port)
        {
            Tuple<string, string> key = new Tuple<string, string>(hostname, port);
            socketclients.TryGetValue(key,out SocketClient socketclient);
            if (socketclient == null)
            {
                try
                {
                    socketclient = new SocketClient(key.Item1, key.Item2);
                    socketclient.Connect();
                    socketclients[key] = socketclient;
                }
                catch (SocketException err)
                {
                    if (!socketclient.Reconnect())
                    {
                        throw err;
                    }
                }
            }
            return socketclient;
        }
        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="serverIp">远程服务IP</param>
        /// <param name="port">远程服务端口</param>
        /// <returns>客户端</returns>
        public static SocketClient GetClient(Tuple<string, string> key)
        {
            SocketClient socketclient;
            socketclients.TryGetValue(key, out socketclient);
            return socketclient;
        }
    }
}
