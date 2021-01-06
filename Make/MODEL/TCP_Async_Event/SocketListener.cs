using Make.MODEL.RPC;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Make.MODEL.TCP_Async_Event
{

    public sealed class SocketListener : IDisposable
    {
        private Socket listenSocket;

        private static Mutex mutex = new Mutex(false);

        private int bufferSize;

        private int numConnectedSockets;

        private int numConnections;

        private SocketAsyncEventArgsPool readWritePool;

        private Semaphore semaphoreAcceptedClients;

        AutoResetEvent keepalive = new AutoResetEvent(false);

        string hostname;

        int port;
        public SocketListener(string hostname,string port,int numConnections, int bufferSize)
        {
            this.numConnectedSockets = 0;
            this.numConnections = numConnections;
            this.bufferSize = bufferSize;
            this.hostname = hostname;
            this.port = int.Parse(port);
            this.readWritePool = new SocketAsyncEventArgsPool(numConnections);
            this.semaphoreAcceptedClients = new Semaphore(numConnections, numConnections);


            for (int i = 0; i < this.numConnections; i++)
            {
                SocketAsyncEventArgs readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += OnReceiveCompleted;
                readWriteEventArg.UserToken = new Token(bufferSize, hostname, port);
                readWriteEventArg.SetBuffer(new Byte[this.bufferSize], 0, this.bufferSize);
                this.readWritePool.Push(readWriteEventArg);
            }


            IPAddress[] addressList = Dns.GetHostEntry(hostname).AddressList;


            IPEndPoint localEndPoint = new IPEndPoint(addressList[addressList.Length - 1],int.Parse(port));


            this.listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocket.ReceiveBufferSize = this.bufferSize;
            this.listenSocket.SendBufferSize = this.bufferSize;
            if (localEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                this.listenSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                this.listenSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, localEndPoint.Port));
            }
            else
            {

                this.listenSocket.Bind(localEndPoint);
            }


            this.listenSocket.Listen(this.numConnections);


        }


        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            try
            {
                e.AcceptSocket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                
            }
            e.AcceptSocket.Close();
            e.AcceptSocket.Dispose();
            e.AcceptSocket = null;

            this.semaphoreAcceptedClients.Release();
            Interlocked.Decrement(ref this.numConnectedSockets);

            this.readWritePool.Push(e);
            Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", this.numConnectedSockets);
        }
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            Socket s = e.AcceptSocket;
            try
            {
                if (s.Connected)
                {
                    SocketAsyncEventArgs readEventArgs = this.readWritePool.Pop();
                    if (readEventArgs != null)
                    {
                        // Get the socket for the accepted client connection and put it into the 
                        // ReadEventArg object user token.
                        readEventArgs.AcceptSocket = s;
                        Token token = (readEventArgs.UserToken as Token);
                        token.Init(s);
                        Interlocked.Increment(ref this.numConnectedSockets);
                        Console.WriteLine("Client connection accepted. There are {0} clients connected to the server",
                            this.numConnectedSockets);
                        if (!s.ReceiveAsync(readEventArgs))
                        {
                            ProcessReceive(readEventArgs);
                        }
                    }
                    else
                    {
                        Console.WriteLine("There are no more available sockets to allocate.");
                    }
                }
                else throw new SocketException((int)SocketError.NotConnected);
            }
            catch (SocketException ex)
            {
                Token token = e.UserToken as Token;
                Console.WriteLine("Error when processing data received from {0}:\r\n{1}", token.Connection.RemoteEndPoint, ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                keepalive.Set();
            }
        }

        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            Console.WriteLine($"[线程]{Thread.CurrentThread.Name}:{hostname}:{port}线程任务已经开始运行");
            while (true)
            {
                mutex.WaitOne();
                if (acceptEventArg == null)
                {
                    acceptEventArg = new SocketAsyncEventArgs();
                    acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                }
                else
                {
                    // Socket must be cleared since the context object is being reused.
                    acceptEventArg.AcceptSocket = null;
                }
                this.semaphoreAcceptedClients.WaitOne();
                Console.WriteLine($"[线程]{Thread.CurrentThread.Name}:开始异步等待{hostname}:{port}中Accpet请求");
                if (!this.listenSocket.AcceptAsync(acceptEventArg))
                {
                    this.ProcessAccept(acceptEventArg);
                }
                else
                {
                    keepalive.Reset();
                    keepalive.WaitOne();
                }
                Console.WriteLine($"[线程]{Thread.CurrentThread.Name}:完成{hostname}:{port}中请求的Accpet");
                mutex.ReleaseMutex();
                Thread.Sleep(50);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            // Check if the remote host closed the connection.
            if (e.BytesTransferred > 0)
            {
                if (e.SocketError == SocketError.Success)
                {
                    Token token = e.UserToken as Token;
                    token.SetData(e);
                    if (!token.Connection.ReceiveAsync(e))
                    {
                        // Read the next block of data sent by client.
                        this.ProcessReceive(e);
                    }
                }
                else
                {
                    CloseClientSocket(e);
                }
            }   
            else
            {
                CloseClientSocket(e);
            }
        }
        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessReceive(e);
        }
        public void Stop()
        {
            this.listenSocket.Close();
            mutex.ReleaseMutex();
        }

        #region IDisposable Members
        bool isDipose = false;

        ~SocketListener()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Console.WriteLine($"{Thread.CurrentThread.Name}开始销毁{hostname}:{port}实例");
            if (isDipose) return;
            if (disposing)
            {
                semaphoreAcceptedClients = null;
                readWritePool = null;
            }
            //处理非托管资源
            try
            {
                listenSocket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception)
            {
                // Throw if client has closed, so it is not necessary to catch.
            }
            finally
            {
                listenSocket.Close();
                listenSocket = null;
            }
            isDipose = true;
        }
        #endregion
    }
}
