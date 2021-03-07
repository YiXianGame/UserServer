using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Material.EtherealC.Model;
using Material.EtherealC.Service;
using Newtonsoft.Json;

namespace Material.EtherealC.Net.AsyncClient
{
    /// <summary>
    /// Token for use with SocketAsyncEventArgs.
    /// </summary>
    public sealed class Token
    {
        //下面两部分只负责接收部分，发包构造部分并没有使用，修改时请注意！
        //下面这部分用于拆包分析   
        private static int headsize = 32;//头包长度
        private static int bodysize = 4;//数据大小长度
        private static int patternsize = 1;//消息类型长度
        private static int futuresize = 27;//后期看情况加
        //下面这部分的byte用于接收数据
        private static byte pattern;
        private static byte[] future = new byte[futuresize];
        ConcurrentDictionary<int, ClientRequestModel> tasks = new ConcurrentDictionary<int, ClientRequestModel>();
        Random random = new Random();
        string hostname;
        string port;
        private SocketAsyncEventArgs socketArgs;
        private DotNetty.Buffers.IByteBuffer content;
        private int needRemain;
        private RPCNetConfig config;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="connection">Socket to accept incoming data.</param>
        /// <param name="capacity">Buffer size for accepted data.</param>
        public Token(SocketAsyncEventArgs eventArgs, string hostname, string port,RPCNetConfig config)
        {
            this.socketArgs = eventArgs;
            this.content = DotNetty.Buffers.UnpooledByteBufferAllocator.Default.DirectBuffer(eventArgs.Buffer.Length, 1024000);
            this.hostname = hostname;
            this.port = port;
            this.config = config;
        }
        public void Init()
        {
            content.ResetWriterIndex();
        }
        public void ProcessData()
        {
            int writerIndex = socketArgs.BytesTransferred + socketArgs.Offset;
            int readerIndex = 0;
            while (readerIndex < writerIndex)
            {
                //存在断包
                if (needRemain != 0)
                {
                    //如果接收数据满足整条量
                    if (needRemain <= writerIndex - readerIndex)
                    {
                        content.WriteBytes(socketArgs.Buffer, readerIndex, needRemain);
                        string data = content.GetString(0, content.WriterIndex, Encoding.UTF8);
                        content.ResetWriterIndex();
                        readerIndex = needRemain + readerIndex;
                        needRemain = 0;
                        //0-Request 1-Response
                        if (pattern == 0)
                        {
                            ServerRequestModel request = JsonConvert.DeserializeObject<ServerRequestModel>(data);
                            if (!RPCServiceFactory.services.TryGetValue(new Tuple<string, string, string>(request.Service, hostname, port), out RPCService proxy) || !proxy.Methods.TryGetValue(request.MethodId, out MethodInfo method))
                            {
#if DEBUG
                                Console.WriteLine("------------------未找到该适配--------------------");
                                Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[客]\n{request}");
                                Console.WriteLine("------------------未找到该适配--------------------");
#endif
                            }
                            else
                            {
#if DEBUG
                                Console.WriteLine("---------------------------------------------------------");
                                Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[服-指令]\n{request}");
                                Console.WriteLine("---------------------------------------------------------");
#endif
                                proxy.ConvertParams(request.MethodId, request.Params);
                                method.Invoke(proxy.Instance, request.Params);
                            }
                        }
                        else
                        {
                            ClientResponseModel response = JsonConvert.DeserializeObject<ClientResponseModel>(data);
#if DEBUG
                            Console.WriteLine("---------------------------------------------------------");
                            Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[服-返回]\n{response}");
                            Console.WriteLine("---------------------------------------------------------");
#endif
                            if (int.TryParse(response.Id, out int id) && tasks.TryGetValue(id, out ClientRequestModel request))
                            {
                                request.set(response);
                            }
                        }
                        continue;
                    }
                    else
                    {
                        int remain = writerIndex - readerIndex;
                        content.WriteBytes(socketArgs.Buffer, readerIndex, remain);
                        needRemain -= remain;
                        break;
                    }
                }
                else
                {
                    int remain = writerIndex - readerIndex;
                    //头包凑不齐，直接返回等待下一次数据
                    if (remain < headsize)
                    {
                        Buffer.BlockCopy(socketArgs.Buffer, readerIndex, socketArgs.Buffer, 0, remain);
                        socketArgs.SetBuffer(remain, socketArgs.Buffer.Length - remain);
                        return;
                    }
                    else
                    {
                        //收到头包，开始对头包拆分
                        //4个字节的数据大小
                        needRemain = BitConverter.ToInt32(socketArgs.Buffer, readerIndex);
                        //1个字节的接收方式
                        pattern = socketArgs.Buffer[readerIndex + bodysize];
                        //接收剩下的27个不用的字节
                        Buffer.BlockCopy(socketArgs.Buffer, readerIndex + bodysize + patternsize, future, 0, futuresize);
                        readerIndex += headsize;
                        continue;
                    }
                }
            }
            socketArgs.SetBuffer(0, socketArgs.Buffer.Length);
        }

        //比较过栈、自增随机数、二次随机数，另外并没有出现网上那种随机数极短时间内出现两次相同值的情况
        //一般收发数量也不会很多，1e4以内随机数要比栈快很多.
        public void Send(ClientRequestModel request)
        {
            if (socketArgs.AcceptSocket != null && socketArgs.AcceptSocket.Connected)
            {
#if DEBUG
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[客-请求]\n{request}");
                Console.WriteLine("---------------------------------------------------------"); 
#endif
                int id = random.Next();
                while (tasks.TryGetValue(id, out ClientRequestModel value))
                {
                    id = random.Next();
                }
                request.Id = id.ToString();
                tasks.TryAdd(id, request);
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
                socketArgs.AcceptSocket.SendAsync(sendEventArgs);
            }
        }
        public void SendVoid(ClientRequestModel request)
        {
            if (socketArgs.AcceptSocket != null && socketArgs.AcceptSocket.Connected)
            {
#if DEBUG
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[客-指令]\n{request}");
                Console.WriteLine("--------------------------------------------------"); 
#endif
                //构造data数据
                byte[] bodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
                //构造表头数据，固定4个字节的长度，表示内容的长度
                byte[] headerBytes = BitConverter.GetBytes(bodyBytes.Length);
                //构造消息类型 0 为Respond,1 为VoidRespond
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
                socketArgs.AcceptSocket.SendAsync(sendEventArgs);
            }
        }
    }
}
