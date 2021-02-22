using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Material.RPCServer.TCP_Async_Event
{
    public abstract class BaseUserToken 
    {
        #region --委托--
        public delegate BaseUserToken GetInstance();
        public delegate void ConnectDelegate();
        public delegate void ClearDelegate();
        #endregion

        #region --事件--
        public event ConnectDelegate ConnectEvent;
        public event ClearDelegate ClearEvent;
        #endregion

        #region --字段
        string hostname;
        string port;
        SocketAsyncEventArgs eventArgs;
        #endregion

        #region --属性--

        public string Hostname { get => hostname; set => hostname = value; }
        public string Port { get => port; set => port = value; }
        internal SocketAsyncEventArgs EventArgs { get => eventArgs; set => eventArgs = value; }
        public bool Connected
        { 
            get 
            {
                if (eventArgs.AcceptSocket != null) return eventArgs.AcceptSocket.Connected;
                else return false;
            } 
        }
        #endregion
        #region --方法--
        internal void Send(ServerRequestModel request)
        {
            if (EventArgs.AcceptSocket.Connected)
            {
#if DEBUG
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"{DateTime.Now}::{Hostname}:{Port}::[服-指令]\n{request}");
                Console.WriteLine("---------------------------------------------------------");
#endif
                //构造data数据
                byte[] bodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
                //构造表头数据，固定4个字节的长度，表示内容的长度
                byte[] headerBytes = BitConverter.GetBytes(bodyBytes.Length);
                //构造消息类型 0 为Respond,1 为Request
                byte[] pattern = { 0 };
                //预备未来的一些数据
                byte[] future = new byte[27];
                //总计需要
                byte[] sendBuffer = new byte[headerBytes.Length + pattern.Length + future.Length + bodyBytes.Length];
                ///拷贝到同一个byte[]数组中
                Buffer.BlockCopy(headerBytes, 0, sendBuffer, 0, headerBytes.Length);
                Buffer.BlockCopy(pattern, 0, sendBuffer, headerBytes.Length, pattern.Length);
                Buffer.BlockCopy(future, 0, sendBuffer, headerBytes.Length + pattern.Length, future.Length);
                Buffer.BlockCopy(bodyBytes, 0, sendBuffer, headerBytes.Length + pattern.Length + future.Length, bodyBytes.Length);
                SocketAsyncEventArgs sendEventArgs = new SocketAsyncEventArgs();
                sendEventArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);
                EventArgs.AcceptSocket.SendAsync(sendEventArgs);
            }
            else
            {
                throw new SocketException((Int32)SocketError.NotConnected);
            }
        }
        internal void Send(ClientResponseModel response)
        {
            if (EventArgs.AcceptSocket.Connected)
            {
#if DEBUG
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"{DateTime.Now}::{Hostname}:{Port}::[客-返回]\n{response}");
                Console.WriteLine("---------------------------------------------------------");
#endif
                //构造data数据
                byte[] bodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
                //构造表头数据，固定4个字节的长度，表示内容的长度
                byte[] headerBytes = BitConverter.GetBytes(bodyBytes.Length);
                //构造消息类型 1 为Respond,0 为Request
                byte[] pattern = { 1 };
                //预备未来的一些数据
                byte[] future = new byte[27];
                //总计需要
                byte[] sendBuffer = new byte[headerBytes.Length + pattern.Length + future.Length + bodyBytes.Length];
                ///拷贝到同一个byte[]数组中
                Buffer.BlockCopy(headerBytes, 0, sendBuffer, 0, headerBytes.Length);
                Buffer.BlockCopy(pattern, 0, sendBuffer, headerBytes.Length, pattern.Length);
                Buffer.BlockCopy(future, 0, sendBuffer, headerBytes.Length + pattern.Length, future.Length);
                Buffer.BlockCopy(bodyBytes, 0, sendBuffer, headerBytes.Length + pattern.Length + future.Length, bodyBytes.Length);
                SocketAsyncEventArgs sendEventArgs = new SocketAsyncEventArgs();
                sendEventArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);
                EventArgs.AcceptSocket.SendAsync(sendEventArgs);
            }
        }

        public virtual void Init()
        {

        }
        public virtual object GetKey()
        {
            return -1;
        }
        public bool AddIntoTokens()
        {
            return RPCNetServerFactory.GetTokens(new Tuple<string, string>(Hostname,Port)).TryAdd(GetKey(), this);
        }
        public bool RemoveFromTokens()
        {
            return RPCNetServerFactory.GetTokens(new Tuple<string, string>(Hostname, Port)).TryRemove(GetKey(),out BaseUserToken value);
        }
        public ConcurrentDictionary<object, BaseUserToken> GetTokens()
        {
            return RPCNetServerFactory.GetTokens(new Tuple<string, string>(Hostname, Port));
        }
        public bool GetToken(object key,out BaseUserToken value)
        {
            return RPCNetServerFactory.GetTokens(new Tuple<string, string>(Hostname, Port)).TryGetValue(key,out value);
        }
        public static ConcurrentDictionary<object, BaseUserToken> GetTokens(Tuple<string, string> serverkey)
        {
            return RPCNetServerFactory.GetTokens(serverkey);
        }
        internal void OnConnectEvent()
        {
            Init();
            ConnectEvent();
        }
        internal void OnClearEvent()
        {
            Init();
            ClearEvent();
        }
        #endregion
    }
}
