using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Material.EtherealC.Net.AsyncClient;

namespace Material.EtherealC.Net
{
    /// <summary>
    /// 客户端工厂
    /// </summary>
    public class RPCNetFactory
    {
        private static Dictionary<Tuple<string, string>, SocketClient> socketclients { get; } = new Dictionary<Tuple<string, string>, SocketClient>();
        /// <summary>
        /// 启动客户端
        /// </summary>
        /// <param name="serverIp">远程服务IP</param>
        /// <param name="port">远程服务端口</param>
        /// <returns>客户端</returns>
        public static SocketClient StartClient(string host,string port,RPCNetConfig config)
        {
            Tuple<string, string> key = new Tuple<string, string>(host, port);
            socketclients.TryGetValue(key,out SocketClient socketclient);
            if (socketclient == null)
            {
                try
                {
                    socketclient = new SocketClient(host,port,config);
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
