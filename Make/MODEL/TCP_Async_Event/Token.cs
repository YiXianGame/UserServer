using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Globalization;
using System.Diagnostics;
using System.Threading.Tasks;
using Make.MODEL;
using Newtonsoft.Json;
using Make.MODEL.RPC;
using System.Reflection;

namespace Make.MODEL.TCP_Async_Event
{
    public sealed class Token
    {

        private Socket connection;

        private StringBuilder sb;

        private int needRemain;

        private User user;
        string hostname;
        string port;
        //下面两部分只负责接收部分，发包构造部分并没有使用，修改时请注意！
        //下面这部分用于拆包分析   
        const int headsize = 32;//头包长度
        const int bodysize = 4;//数据大小长度
        const int patternsize = 1;//消息类型长度
        const int futuresize = 27;//后期看情况加
        //下面这部分的byte用于接收数据
        private byte[] head = new byte[headsize + 1];//留一个字节表示头包数据已接收大小
        private byte[] pattern = new byte[patternsize];
        private byte[] future = new byte[futuresize];
        public Token(int bufferSize,string hostname,string port)
        {
            this.sb = new StringBuilder(bufferSize);
            this.hostname = hostname;
            this.port = port;
        }
        public void Init(Socket socket)
        {
            connection = socket;
            sb.Length = 0;
            head[0] = 0;
        }
        public Socket Connection
        {
            get { return this.connection; }
            set { connection = value; }
        }

        public User User { get => user; set => user = value; }

        public string ProcessData(SocketAsyncEventArgs args)
        {
            String received = this.sb.ToString();
            sb.Length = 0;
            needRemain = 0;
            return received;
        }

        public void SetData(SocketAsyncEventArgs args)
        {
            int count = args.BytesTransferred;
            int pos = 0;
            while (pos < count)
            {
                //存在断包
                if (needRemain != 0)
                {
                    //如果接收数据满足整条量
                    if (needRemain <= count - pos)
                    {
                        string data = Encoding.UTF8.GetString(args.Buffer, pos, needRemain);
                        //从客户端发回来的，只可能是请求，绝对不会是响应，因为服务器绝对不会因为一个客户进行一个线程等待.
                        ClientRequestModel request = JsonConvert.DeserializeObject<ClientRequestModel>(data);
                        RPCAdaptFactory.services.TryGetValue(new Tuple<string, string, string>(request.Service, hostname, port), out RPCAdaptProxy proxy);
                        proxy.methods.TryGetValue(request.Methodid, out MethodInfo method);
                        //0-Response 1-VoidRespond
                        if (pattern[0] == 0)
                        {
#if DEBUG
                            Console.WriteLine("--------------------------------------------------");
                            Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[客-请求]\n{request.ToString()}");
                            Console.WriteLine("--------------------------------------------------"); 
#endif
                            request.Params[0] = this;
                            Task.Run(() => {
                                Send(new ClientResponseModel("2.0", method.Invoke(null, request.Params), new Error(), request.ID));
                            });
                            //也可以直接调用，看实际情况，是打算开Task处理还是就在接收线程中处理，前者节约接收信息的时间，后者节约资源
                            //Send(new ClientResponseModel("2.0", method.Invoke(null, request.Params), new Error(), request.ID));
                        }
                        else
                        {
#if DEBUG
                            Console.WriteLine("--------------------------------------------------");
                            Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[客-指令]\n{data.ToString()}");
                            Console.WriteLine("--------------------------------------------------"); 
#endif
                            request.Params[0] = this;
                            //也可以直接调用，看实际情况，是打算开Task处理还是就在接收线程中处理，前者节约接收信息的时间，后者节约资源
                            //method.Invoke(null, request.Params);
                            Task.Run(() => {
                                method.Invoke(null, request.Params);
                            });
                        }
                        pos = needRemain + pos;
                        needRemain = 0;
                        continue;
                    }
                    else
                    {
                        sb.Append(Encoding.UTF8.GetString(args.Buffer, pos, count - pos));
                        needRemain = needRemain - count + pos;
                        break;
                    }
                }
                else
                {
                    //头包凑不齐，直接返回等待下一次数据
                    if (count - pos < headsize - head[0])
                    {
                        Buffer.BlockCopy(args.Buffer, pos, head, head[0] + 1, count - pos);
                        head[0] += (byte)(count - pos);
                        break;
                    }
                    else
                    {
                        Buffer.BlockCopy(args.Buffer, pos, head, head[0] + 1, headsize - head[0]);
                        pos += headsize - head[0];
                        head[0] = 0;
                        //收到头包，开始对头包拆分
                        //4个字节的数据大小
                        needRemain = BitConverter.ToInt32(head, 1);
                        //1个字节的接收方式
                        pattern[0] = head[bodysize + 1];
                        //接收剩下的27个不用的字节
                        Buffer.BlockCopy(head, bodysize + patternsize + 1, future, 0, futuresize);
                        continue;
                    }
                }
            }
        }


        public void Send(ServerRequestModel request)
        {
            if (Connection.Connected)
            {
#if DEBUG
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[服-指令]\n{request.ToString()}");
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
                Connection.SendAsync(sendEventArgs);
            }
            else
            {
                throw new SocketException((Int32)SocketError.NotConnected);
            }
        }
        public void Send(ClientResponseModel response)
        {
            if (Connection.Connected)
            {
#if DEBUG
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[服-返回]\n{response.ToString()}");
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
                Connection.SendAsync(sendEventArgs);
            }
        }
    }
}
