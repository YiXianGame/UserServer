using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Globalization;
using System.Diagnostics;
using System.Threading.Tasks;
using Make.MODEL;
using Newtonsoft.Json;

namespace Make.MODEL.TCP_Async_Event
{
    delegate void ProcessData(SocketAsyncEventArgs args);
    /// <summary>
    /// Token for use with SocketAsyncEventArgs.
    /// </summary>
    public sealed class Token : IDisposable
    {

        private Socket connection;

        private StringBuilder sb;

        private Int32 needRemain;

        private byte[] head = new byte[5];

        private MsgToken msgToken;

        private User user;
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="connection">Socket to accept incoming data.</param>
        /// <param name="bufferSize">Buffer size for accepted data.</param>
        internal Token(Socket connection, Int32 bufferSize)
        {
            this.connection = connection;
            this.sb = new StringBuilder(bufferSize);
        }

        /// <summary>
        /// Accept socket.
        /// </summary>
        internal Socket Connection
        {
            get { return this.connection; }
        }

        public MsgToken MsgToken { get => msgToken; set => msgToken = value; }
        public User User { get => user; set => user = value; }

        /// <summary>
        /// Process data received from the client.
        /// </summary>
        /// <param name="args">SocketAsyncEventArgs used in the operation.</param>
        internal string ProcessData(SocketAsyncEventArgs args)
        {
            // Get the message received from the client.
            String received = this.sb.ToString();

            //TODO Use message received to perform a specific operation.

            // Clear StringBuffer, so it can receive more data from a keep-alive connection client.
            sb.Length = 0;
            needRemain = 0;
            return received;
        }

        /// <summary>
        /// Set data received from the client.
        /// </summary>
        /// <param name="args">SocketAsyncEventArgs used in the operation.</param>
        internal void SetData(SocketAsyncEventArgs args)
        {
            int count = args.BytesTransferred;
            int p = 0;
            //存在断包
            if (needRemain != 0)
            {
                if (needRemain <= count)
                {
                    Debug.WriteLine($"[足够] 缓冲区大小{args.Buffer.Length} 总量{count}\n待接收量{needRemain}");
                    sb.Append(Encoding.UTF8.GetString(args.Buffer, 0, needRemain));
                    p = needRemain;
                    string data = ProcessData(args);
                    //处理本次数据
                    Task.Run(() => TCP_Event.OnReceive(this,JsonConvert.DeserializeObject<Msg_Client>(data)));
                }
                else
                {
                    Debug.WriteLine($"[不足] 缓冲区大小{args.Buffer.Length} 总量{count}\n待接收量{needRemain}");
                    sb.Append(Encoding.UTF8.GetString(args.Buffer, 0, count));
                    needRemain -= count;
                    return;
                }
            }
            while (p < count)
            {
                //头包凑不齐，直接返回等待下一次数据
                if (count - p < 4 - head[0])
                {
                    Buffer.BlockCopy(args.Buffer, p, head, head[0] + 1, count - p);
                    head[0] += (byte)(count - p);
                    return;
                }
                else
                {
                    Buffer.BlockCopy(args.Buffer, p, head, head[0] + 1, 4 - head[0]);
                    p += 4 - head[0];
                    head[0] = (byte)0;
                }
                //收到头包
                needRemain = BitConverter.ToInt32(head, 1);

                //如果接收数据满足整条量
                if (needRemain <= count - p)
                {
                    string data = Encoding.UTF8.GetString(args.Buffer, p, needRemain);
                    //处理本次数据    
                    Task.Run(() => TCP_Event.OnReceive(this,JsonConvert.DeserializeObject<Msg_Client>(data)));
                    p = needRemain + p;
                    needRemain = 0;
                }
                else
                {
                    sb.Append(Encoding.UTF8.GetString(args.Buffer, p, count - p));
                    needRemain = needRemain - count + p;
                    break;
                }
            }
        }

        /// <summary>
        /// 构造发送数据
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public byte[] Convert_SendMsg(string msg)
        {
            int length = msg.Length;
            //构造内容
            byte[] bodyBytes = Encoding.UTF8.GetBytes(msg);
            //构造表头数据，固定4个字节的长度，表示内容的长度
            byte[] headerBytes = BitConverter.GetBytes(bodyBytes.Length);

            byte[] tempBytes = new byte[headerBytes.Length + bodyBytes.Length];
            ///拷贝到同一个byte[]数组中，发送出去..
            Buffer.BlockCopy(headerBytes, 0, tempBytes, 0, headerBytes.Length);
            Buffer.BlockCopy(bodyBytes, 0, tempBytes, headerBytes.Length, bodyBytes.Length);

            return tempBytes;
        }
        public void Send(Enums.Msg_Server_Type type, String message, object bound = null)
        {
            if (this.Connection.Connected)
            {
                // Create a buffer to send.
                Msg_Server msg_Server = new Msg_Server(MsgToken, type, message, JsonConvert.SerializeObject(bound));
                Byte[] sendBuffer = Convert_SendMsg(JsonConvert.SerializeObject(msg_Server));
                // Prepare arguments for send/receive operation.
                SocketAsyncEventArgs completeArgs = new SocketAsyncEventArgs();
                completeArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);
                completeArgs.UserToken = this.Connection;
                completeArgs.RemoteEndPoint = this.Connection.RemoteEndPoint;
                // Start sending asyncronally.
                this.Connection.SendAsync(completeArgs);
            }
            else
            {
                throw new SocketException((Int32)SocketError.NotConnected);
            }
        }


        #region IDisposable Members

        /// <summary>
        /// Release instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.connection.Shutdown(SocketShutdown.Send);
            }
            catch (Exception)
            {
                // Throw if client has closed, so it is not necessary to catch.
            }
            finally
            {
                this.connection.Close();
            }
        }

        #endregion
    }
}
