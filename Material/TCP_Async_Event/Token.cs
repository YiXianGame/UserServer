using System;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using Material.RPC;

namespace Material.TCP_Async_Event
{
    public sealed class Token
    {
        private SocketAsyncEventArgs eventArgs;
        private DotNetty.Buffers.IByteBuffer content;
        private int needRemain;
        string hostname;
        string port;
        //下面两部分只负责接收部分，发包构造部分并没有使用，修改时请注意！
        //下面这部分用于拆包分析   
        private static int headsize = 32;//头包长度
        private static int bodysize = 4;//数据大小长度
        private static int patternsize = 1;//消息类型长度
        private static int futuresize = 27;//后期看情况加
        //下面这部分的byte用于接收数据
        private static byte pattern;
        private static byte[] future = new byte[futuresize];
        public Token(SocketAsyncEventArgs eventArgs,string hostname,string port)
        {
            this.eventArgs = eventArgs;
            this.content = DotNetty.Buffers.UnpooledByteBufferAllocator.Default.DirectBuffer(eventArgs.Buffer.Length,1024000);
            this.hostname = hostname;
            this.port = port;
        }
        public void Init()
        {
            content.ResetWriterIndex();
        }

        public void ProcessData()
        {
            int writerIndex = eventArgs.BytesTransferred + eventArgs.Offset;
            int readerIndex = 0;
            while (readerIndex < writerIndex)
            {
                //存在断包
                if (needRemain != 0)
                {
                    //如果接收数据满足整条量
                    if (needRemain <= writerIndex - readerIndex)
                    {
                        content.WriteBytes(eventArgs.Buffer, readerIndex, needRemain);
                        //从客户端发回来的，只可能是请求，绝对不会是响应，因为服务器绝对不会因为一个客户进行一个线程等待.
                        ClientRequestModel request = JsonConvert.DeserializeObject<ClientRequestModel>(content.GetString(0,content.WriterIndex, Encoding.UTF8));
                        content.ResetWriterIndex();
                        if(!RPCAdaptFactory.services.TryGetValue(new Tuple<string, string, string>(request.Service, hostname, port), out RPCAdaptProxy proxy) || !proxy.Methods.TryGetValue(request.MethodId, out MethodInfo method))
                        {
#if DEBUG
                            Console.WriteLine("------------------未找到该方法--------------------");
                            Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[客]\n{request}");
                            Console.WriteLine("------------------未找到该方法--------------------");
#endif                      
                        }
                        else
                        {
                            proxy.ConvertParams(request.MethodId,request.Params);
                            //0-Request 1-Command
                            if (pattern == 0)
                            {
#if DEBUG
                                Console.WriteLine("--------------------------------------------------");
                                Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[客-请求]\n{request}");
                                Console.WriteLine("--------------------------------------------------");
#endif
                                request.Params[0] = this;
                                Send(new ClientResponseModel("2.0", method.Invoke(null, request.Params), new Error(), request.Id));
                            }
                            else
                            {
#if DEBUG
                                Console.WriteLine("--------------------------------------------------");
                                Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[客-指令]\n{request}");
                                Console.WriteLine("--------------------------------------------------");
#endif
                                request.Params[0] = this;
                                //也可以直接调用，看实际情况，是打算开Task处理还是就在接收线程中处理，前者节约接收信息的时间、一条线程并发处理消息，后者节约资源
                                //method.Invoke(null, request.Params);
                                Task.Run(() => {
                                    method.Invoke(null, request.Params);
                                });
                            }
                        }
                        readerIndex = needRemain + readerIndex;
                        needRemain = 0;
                        continue;
                    }
                    else
                    {
                        int remain = writerIndex - readerIndex;
                        content.WriteBytes(eventArgs.Buffer, readerIndex, remain);
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
                        Buffer.BlockCopy(eventArgs.Buffer, readerIndex,eventArgs.Buffer,0,remain);
                        eventArgs.SetBuffer(remain,eventArgs.Buffer.Length - remain);
                        return;
                    }
                    else
                    {
                        //收到头包，开始对头包拆分
                        //4个字节的数据大小
                        needRemain = BitConverter.ToInt32(eventArgs.Buffer, readerIndex);
                        //1个字节的接收方式
                        pattern = eventArgs.Buffer[readerIndex + bodysize];
                        //接收剩下的27个不用的字节
                        Buffer.BlockCopy(eventArgs.Buffer, readerIndex + bodysize + patternsize, future, 0, futuresize);
                        readerIndex += headsize;
                        continue;
                    }
                }
            }
            eventArgs.SetBuffer(0, eventArgs.Buffer.Length);
        }


        public void Send(ServerRequestModel request)
        {
            if (eventArgs.AcceptSocket.Connected)
            {
#if DEBUG
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[服-指令]\n{request}");
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
                eventArgs.AcceptSocket.SendAsync(sendEventArgs);
            }
            else
            {
                throw new SocketException((Int32)SocketError.NotConnected);
            }
        }
        public void Send(ClientResponseModel response)
        {
            if (eventArgs.AcceptSocket.Connected)
            {
#if DEBUG
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"{DateTime.Now}::{hostname}:{port}::[服-返回]\n{response}");
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
                eventArgs.AcceptSocket.SendAsync(sendEventArgs);
            }
        }
    }
}
