using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Material.EtherealS.Model
{
    public abstract class BaseUserToken
    {
        #region --委托--
        public delegate BaseUserToken GetInstance();
        #endregion

        #region --事件--

        #endregion

        #region --字段--
        private Tuple<string, string> serverKey;
        private Socket socket;
        #endregion

        #region --属性--

        public Tuple<string, string> ServerKey { get => serverKey; set => serverKey = value; }
        public Socket Socket { get => socket; set => socket = value; }
        #endregion

        #region --方法--
        internal void Send(ServerRequestModel request)
        {
            if (socket.Connected)
            {
#if DEBUG
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"{DateTime.Now}::{serverKey.Item1}:{serverKey.Item2}::[服-指令]\n{request}");
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
                socket.SendAsync(sendEventArgs);
            }
            else
            {
                throw new SocketException((Int32)SocketError.NotConnected);
            }
        }
        internal void Send(ClientResponseModel response)
        {
            if (socket.Connected)
            {
#if DEBUG
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"{DateTime.Now}::{serverKey.Item1}:{serverKey.Item2}::[服-返回]\n{response}");
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
                socket.SendAsync(sendEventArgs);
            }
        }
        public bool AddIntoTokens(bool replace = false)
        {
            if (replace)
            {
                Net.RPCNetFactory.GetTokens(serverKey).TryRemove(Key, out BaseUserToken token);
                return Net.RPCNetFactory.GetTokens(serverKey).TryAdd(Key, this);
            }
            else return Net.RPCNetFactory.GetTokens(serverKey).TryAdd(Key, this);
        }
        public bool RemoveFromTokens()
        {
            return Net.RPCNetFactory.GetTokens(serverKey).TryRemove(Key, out BaseUserToken value);
        }
        public ConcurrentDictionary<object, BaseUserToken> GetTokens()
        {
            return Net.RPCNetFactory.GetTokens(serverKey);
        }
        public bool GetToken<T>(object key,out T value) where T:BaseUserToken
        {
            if (Net.RPCNetFactory.GetTokens(serverKey).TryGetValue(key, out BaseUserToken result))
            {
                value = (T)result;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
        public static ConcurrentDictionary<object, BaseUserToken> GetTokens(Tuple<string, string> serverkey)
        {
            return Net.RPCNetFactory.GetTokens(serverkey);
        }



        #endregion

        #region --抽象属性--
        public abstract object Key { get; set; }
        #endregion

        #region --虚方法--
        public virtual void OnConnect()
        {

        }

        public virtual void OnDisConnect()
        {

        }
        #endregion
    }
}
