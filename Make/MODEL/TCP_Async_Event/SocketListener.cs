using Make.MODEL.RPC;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Make.MODEL.TCP_Async_Event
{
    /// <summary>
    /// Based on example from http://msdn2.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.aspx
    /// Implements the connection logic for the socket server.  
    /// After accepting a connection, all data read from the client is sent back. 
    /// The read and echo back to the client pattern is continued until the client disconnects.
    /// </summary>
    public sealed class SocketListener : IDisposable
    {
        /// <summary>
        /// The socket used to listen for incoming connection requests.
        /// </summary>
        private Socket listenSocket;

        /// <summary>
        /// Mutex to synchronize server execution.
        /// </summary>
        private static Mutex mutex = new Mutex(false);

        /// <summary>
        /// Buffer size to use for each socket I/O operation.
        /// </summary>
        private int bufferSize;

        /// <summary>
        /// The total number of clients connected to the server.
        /// </summary>
        private int numConnectedSockets;

        /// <summary>
        /// the maximum number of connections the sample is designed to handle simultaneously.
        /// </summary>
        private int numConnections;

        /// <summary>
        /// Pool of reusable SocketAsyncEventArgs objects for write, read and accept socket operations.
        /// </summary>
        private SocketAsyncEventArgsPool readWritePool;

        /// <summary>
        /// Controls the total number of clients connected to the server.
        /// </summary>
        private Semaphore semaphoreAcceptedClients;

        AutoResetEvent keepalive = new AutoResetEvent(false);

        string hostname;

        int port;
        /// <summary>
        /// Create an uninitialized server instance.  
        /// To start the server listening for connection requests
        /// call the Init method followed by Start method.
        /// </summary>
        /// <param name="numConnections">Maximum number of connections to be handled simultaneously.</param>
        /// <param name="bufferSize">Buffer size to use for each socket I/O operation.</param>
        public SocketListener(string hostname,string port,int numConnections, int bufferSize)
        {
            this.numConnectedSockets = 0;
            this.numConnections = numConnections;
            this.bufferSize = bufferSize;
            this.hostname = hostname;
            this.port = int.Parse(port);
            this.readWritePool = new SocketAsyncEventArgsPool(numConnections);
            this.semaphoreAcceptedClients = new Semaphore(numConnections, numConnections);

            // Preallocate pool of SocketAsyncEventArgs objects.
            for (int i = 0; i < this.numConnections; i++)
            {
                SocketAsyncEventArgs readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += OnReceiveCompleted;
                readWriteEventArg.UserToken = new Token(bufferSize, hostname, port);
                readWriteEventArg.SetBuffer(new Byte[this.bufferSize], 0, this.bufferSize);
                this.readWritePool.Push(readWriteEventArg);
            }

            // Get host related information.
            IPAddress[] addressList = Dns.GetHostEntry(hostname).AddressList;

            // Get endpoint for the listener.
            IPEndPoint localEndPoint = new IPEndPoint(addressList[addressList.Length - 1],int.Parse(port));

            // Create the socket which listens for incoming connections.
            this.listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocket.ReceiveBufferSize = this.bufferSize;
            this.listenSocket.SendBufferSize = this.bufferSize;
            if (localEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                // Set dual-mode (IPv4 & IPv6) for the socket listener.
                // 27 is equivalent to IPV6_V6ONLY socket option in the winsock snippet below,
                // based on http://blogs.msdn.com/wndp/archive/2006/10/24/creating-ip-agnostic-applications-part-2-dual-mode-sockets.aspx
                this.listenSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                this.listenSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, localEndPoint.Port));
            }
            else
            {
                // Associate the socket with the local endpoint.
                this.listenSocket.Bind(localEndPoint);
            }

            // Start the server.
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
                // Throw if client has closed, so it is not necessary to catch.
            }
            e.AcceptSocket.Close();
            e.AcceptSocket.Dispose();
            e.AcceptSocket = null;
            // Decrement the counter keeping track of the total number of clients connected to the server.
            this.semaphoreAcceptedClients.Release();
            Interlocked.Decrement(ref this.numConnectedSockets);
            // Free the SocketAsyncEventArg so they can be reused by another client.
            this.readWritePool.Push(e);
            Console.WriteLine("A client has been disconnected from the server. There are {0} clients connected to the server", this.numConnectedSockets);
        }

        /// <summary>
        /// Callback method associated with Socket.AcceptAsync 
        /// operations and is invoked when an accept operation is complete.
        /// </summary>
        /// <param name="sender">Object who raised the event.</param>
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }


        /// <summary>
        /// Process the accept for the socket listener.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
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


        /// <summary>
        /// Begins an operation to accept a connection request from the client.
        /// </summary>
        /// <param name="acceptEventArg">The context object to use when issuing 
        /// the accept operation on the server's listening socket.</param>
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


        /// <summary>
        /// This method is invoked when an asynchronous receive operation completes. 
        /// If the remote host closed the connection, then the socket is closed.  
        /// If data was received then the data is echoed back to the client.
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed receive operation.</param>
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
        /// <summary>
        /// Stop the server.
        /// </summary>
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
