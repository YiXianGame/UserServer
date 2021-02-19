using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Material.RPCClient.TCP_Async_Event
{

    public sealed class SocketClient : IDisposable
    {
        /// <summary>
        /// Զ�̷�������ַ
        /// </summary>
        private IPEndPoint hostEndPoint;
        /// <summary>
        /// �ź���,�����߳̽������ӵȴ�
        /// </summary>
        private static AutoResetEvent autoConnectEvent = new AutoResetEvent(false);
        /// <summary>
        /// �ͻ�������
        /// </summary>
        private const int DEFAULT_PORT = 28015, DEFAULT_NUM_CONNECTIONS = 1000, DEFAULT_BUFFER_SIZE = 1024;
        private string hostname;
        private string port;
        public SocketAsyncEventArgs SocketArgs { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public SocketClient(string hostname,string port)
        {
            this.hostname = hostname;
            this.port = port;
            // Get host related information.
            IPAddress[] addressList = Dns.GetHostEntry(hostname).AddressList;
            // Get endpoint for the listener.
            IPEndPoint localEndPoint = new IPEndPoint(addressList[addressList.Length - 1],int.Parse(port));
            // Instantiates the endpoint and socket.
            this.hostEndPoint = new IPEndPoint(addressList[addressList.Length - 1], int.Parse(port));
            this.SocketArgs = new SocketAsyncEventArgs();
        }

        public void Connect()
        {
            try
            {
                SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
                acceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnect);
                Socket socket = new Socket(hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                acceptArgs.RemoteEndPoint = hostEndPoint;
                acceptArgs.AcceptSocket = socket;
                socket.ConnectAsync(acceptArgs);
                autoConnectEvent.WaitOne();
                SocketError errorCode = acceptArgs.SocketError;
                if (errorCode == SocketError.Success)
                {
                    SocketArgs = new SocketAsyncEventArgs();
                    SocketArgs.Completed += OnReceiveCompleted;
                    SocketArgs.SetBuffer(new Byte[DEFAULT_BUFFER_SIZE], 0, DEFAULT_BUFFER_SIZE);
                    SocketArgs.AcceptSocket = acceptArgs.AcceptSocket;
                    SocketArgs.UserToken = new Token(SocketArgs, hostname, port);
                    SocketArgs.RemoteEndPoint = this.hostEndPoint;
                    SocketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnect);
                    if (!SocketArgs.AcceptSocket.ReceiveAsync(SocketArgs))
                    {
                        ProcessReceive(SocketArgs);
                    }
                }
                else
                {
                    Reconnect();
                }
            }
            catch(SocketException err)
            {
                Reconnect();
            }
        }

        private void OnConnect(object sender, SocketAsyncEventArgs e)
        {
            autoConnectEvent.Set();
        }

        public void Disconnect()
        {
            SocketArgs.AcceptSocket.Disconnect(false);
        }

        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessReceive(e);
        }
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            // Check if the remote host closed the connection.
            if (e.BytesTransferred > 0)
            {
                if (e.SocketError == SocketError.Success)
                {
                    (e.UserToken as Token).ProcessData();
                    if (!e.AcceptSocket.ReceiveAsync(e))
                    {
                        // Read the next block of data sent by client.
                        this.ProcessReceive(e);
                    }
                }
                else
                {
                    Reconnect();
                }
            }
            else
            {
                Reconnect();
            }
        }
        public bool Reconnect()
        {
            Debug.WriteLine("������������쳣,��ʼ����������");
            Socket clientSocket = SocketArgs.AcceptSocket;
            for (int i = 1; i <= 10; i++)
            {
                if (clientSocket != null)
                {
                    Debug.WriteLine("��ʼ������ʷSocket");
                    clientSocket.Close();
                    clientSocket.Dispose();
                    Debug.WriteLine("��ʷSocket������ɣ�");
                }
                Debug.WriteLine($"��ʼ���е�{i}�γ���");
                clientSocket = new Socket(this.hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
                acceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnect);
                acceptArgs.AcceptSocket = clientSocket;
                acceptArgs.RemoteEndPoint = hostEndPoint;
                try
                {
                    clientSocket.ConnectAsync(acceptArgs);
                    autoConnectEvent.WaitOne();
                    SocketError errorCode = acceptArgs.SocketError;
                    if (errorCode == SocketError.Success)
                    {
                        SocketArgs = new SocketAsyncEventArgs();
                        SocketArgs.SetBuffer(new Byte[DEFAULT_BUFFER_SIZE], 0,DEFAULT_BUFFER_SIZE);
                        SocketArgs.Completed += OnReceiveCompleted;
                        SocketArgs.UserToken = new Token(SocketArgs, hostname, port);
                        if (!clientSocket.ReceiveAsync(SocketArgs))
                        {
                            ProcessReceive(SocketArgs);
                        }
                        Debug.WriteLine("�����ɹ���");
                        break;
                    }
                    else
                    {
                        Debug.WriteLine("����ʧ�ܣ�5������ԣ�");
                        Thread.Sleep(5000);
                    }
                }
                catch (SocketException err)
                {
                    Debug.WriteLine("����ʧ�ܣ�5������ԣ�");
                    Thread.Sleep(5000);
                }
            }
            if (!clientSocket.Connected)
            {
                Debug.WriteLine($"����ʧ�ܣ�");
                return false;
            }
            else return true;
        }
        #region IDisposable Members
        bool isDipose = false;

        ~SocketClient()
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
            Socket clientSocket = SocketArgs.AcceptSocket;
            if (isDipose) return;
            if (disposing)
            {
                hostEndPoint = null;
                autoConnectEvent.Close();
                autoConnectEvent = null;
            }
            //������й���Դ
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
                // Throw if client has closed, so it is not necessary to catch.
            }
            finally
            {
                clientSocket.Close();
                clientSocket = null;
            }
            isDipose = true;
        }
        #endregion
    }
}
