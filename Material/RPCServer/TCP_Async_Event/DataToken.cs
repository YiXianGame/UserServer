using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Material.Entity;
using Newtonsoft.Json;

namespace Material.RPCServer.TCP_Async_Event
{
    public sealed class DataToken
    {
        #region --User_Cutsom--
        private BaseUserToken token;
        #endregion
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

        public BaseUserToken Token  { get => token; set => token = value; }
        public string Hostname { get => hostname; set => hostname = value; }
        public string Port { get => port; set => port = value; }

        public DataToken(SocketAsyncEventArgs eventArgs,string hostname,string port)
        {
            this.eventArgs = eventArgs;
            this.content = DotNetty.Buffers.UnpooledByteBufferAllocator.Default.DirectBuffer(eventArgs.Buffer.Length,1024000);
            this.Hostname = hostname;
            this.Port = port;
        }
        public void DisConnect()
        {
            content.ResetWriterIndex();
            token.OnDisConnect();
            token = null;
        }
        public void Connect(BaseUserToken token,Socket socket)
        {
            content.ResetWriterIndex();
            this.token = token;
            token.Socket = socket;
            token.ServerKey = new Tuple<string, string>(hostname, port);
            token.OnConnect();
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
                        if(!RPCAdaptFactory.services.TryGetValue(new Tuple<string, string, string>(request.Service, Hostname, Port), out RPCAdaptProxy proxy) || !proxy.Methods.TryGetValue(request.MethodId, out MethodInfo method))
                        {   
#if DEBUG
                            Console.WriteLine("------------------未找到该方法--------------------");
                            Console.WriteLine($"{DateTime.Now}::{Hostname}:{Port}::[客]\n{request}");
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
                                Console.WriteLine($"{DateTime.Now}::{Hostname}:{Port}::[客-请求]\n{request}");
                                Console.WriteLine("--------------------------------------------------");
#endif
                                request.Params[0] = token;
                                object result = method.Invoke(proxy.Instance, request.Params);
                                string type = "null";
                                if(result!=null)proxy.Type.AbstractName.TryGetValue(result.GetType(),out type);
                                token.Send(new ClientResponseModel("2.0",JsonConvert.SerializeObject(result),type, new Error(), request.Id));
                            }
                            else
                            {
#if DEBUG
                                Console.WriteLine("--------------------------------------------------");
                                Console.WriteLine($"{DateTime.Now}::{Hostname}:{Port}::[客-指令]\n{request}");
                                Console.WriteLine("--------------------------------------------------");
#endif
                                request.Params[0] = token;
                                method.Invoke(null, request.Params);
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
    }
}
