using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RfidNet
{
    public sealed class IocpTcpSocket:ThreadEx, INetworkSocket, IDisposable
    {
        /// <summary>
        /// actual client 实际连接对象
        /// </summary>
        private TcpClient m_client = null;
        /// <summary>
        /// managing server 管理此对象的服务器
        /// </summary>
        private INetworkServer m_server = null;
        /// <summary>
        /// IP information 地址信息
        /// </summary>
        private IPInfo m_ipInfo;
        /// <summary>
        /// general lock 通用锁
        /// </summary>
        private Object m_generalLock = new Object();
        /// <summary>
        /// send lock 发送锁
        /// </summary>
        private Object m_sendLock = new Object();
        /// <summary>
        /// send queue lock 发送队列
        /// </summary>
        private Object m_sendQueueLock = new Object();
        /// <summary>
        /// send event 发送互斥事件
        /// </summary>
        private EventEx m_sendEvent = new EventEx();
        /// <summary>
        /// callback object 此对象的回调对像
        /// </summary>
        private INetworkSocketCallback m_callBackObj = null;
        /// <summary>
        /// send queue 发送队列
        /// </summary>
        private Queue<BufTransporter> m_sendQueue = new Queue<BufTransporter>();

        //接收客户端发送过来的数据
        private byte[] m_recvData = new byte[2048];

        /// <summary>
        /// flag for connection check 当前连接的状态
        /// </summary>
        private bool m_isConnected = false;

        /// <summary>
        /// flag for no delay delay标志
        /// </summary>
        private bool m_noDelay = true;

        /// <summary>
        /// OnNewConnected event 新的连接委托事件
        /// </summary>
        OnSocketNewConnectionDelegate m_onNewConnection = delegate { };
        /// <summary>
        /// OnRecevied event 接收委托事件
        /// </summary>
        OnSocketReceivedDelegate m_onReceived = delegate { };
        /// <summary>
        /// OnSent event 发送委托事件
        /// </summary>
        OnSocketSentDelegate m_onSent = delegate { };
        /// <summary>
        /// OnDisconnect event 断开委托事件
        /// </summary>
        OnSocketDisconnectDelegate m_onDisconnect = delegate { };


        /// <summary>
        /// OnNewConnected event 新的连接来到事件
        /// </summary>
        public OnSocketNewConnectionDelegate OnNewConnection
        {
            get
            {
                return m_onNewConnection; 
            }
            set
            {
                if (value == null)
                {
                    m_onNewConnection = delegate { };
                    if (CallBackObj != null)
                        m_onNewConnection += CallBackObj.OnNewConnection;
                }
                else
                {
                    m_onNewConnection = CallBackObj != null && CallBackObj.OnNewConnection != value ? CallBackObj.OnNewConnection + (value - CallBackObj.OnNewConnection) : value;
                }
            }
        }

        /// <summary>
        /// OnRecevied event 接收事件
        /// </summary>
        public OnSocketReceivedDelegate OnReceived
        {
            get
            {
                return m_onReceived;
            }
            set
            {
                if (value == null)
                {
                    m_onReceived = delegate { };
                    if (CallBackObj != null)
                        m_onReceived += CallBackObj.OnReceived;
                }
                else
                {
                    m_onReceived = CallBackObj != null && CallBackObj.OnReceived != value ? CallBackObj.OnReceived + (value - CallBackObj.OnReceived) : value;
                }
            }
        }

        /// <summary>
        /// OnSent event 发送事件
        /// </summary>
        public OnSocketSentDelegate OnSent
        {
            get
            {
                return m_onSent;
            }
            set
            {
                if (value == null)
                {
                    m_onSent = delegate { };
                    if (CallBackObj != null)
                        m_onSent += CallBackObj.OnSent;
                }
                else
                {
                    m_onSent = CallBackObj != null && CallBackObj.OnSent != value ? CallBackObj.OnSent + (value - CallBackObj.OnSent) : value;
                }
            }
        }
        /// <summary>
        /// OnDisconnect event 断开事件
        /// </summary>
        public OnSocketDisconnectDelegate OnDisconnect
        {
            get
            {
                return m_onDisconnect;
            }
            set
            {
                if (value == null)
                {
                    m_onDisconnect = delegate { };
                    if (CallBackObj != null)
                        m_onDisconnect += CallBackObj.OnDisconnect;
                }
                else
                {
                    m_onDisconnect = CallBackObj != null && CallBackObj.OnDisconnect != value ? CallBackObj.OnDisconnect + (value - CallBackObj.OnDisconnect) : value;
                }
            }
        }
        /// <summary>
        /// Default constructor 默认构造函数
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="server">managing server</param>
        public IocpTcpSocket(TcpClient client, INetworkServer server) : base()
        {
            m_client = client;
            m_server = server;
            NoDelay = server.NoDelay;
            // 得到客户端IP地址与本地址IP地址信息
            IPEndPoint remoteIpEndPoint = m_client.Client.RemoteEndPoint as IPEndPoint; 
            IPEndPoint localIpEndPoint = m_client.Client.LocalEndPoint as IPEndPoint;
            if (remoteIpEndPoint != null)
            {
                String socketHostName = remoteIpEndPoint.Address.ToString();
                m_ipInfo = new IPInfo(socketHostName, remoteIpEndPoint, IPEndPointType.REMOTE);
            }
            else if (localIpEndPoint != null)
            {
                String socketHostName = localIpEndPoint.Address.ToString();
                m_ipInfo = new IPInfo(socketHostName, localIpEndPoint, IPEndPointType.LOCAL);
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~IocpTcpSocket()
        {
            Dispose(false);
        }

        /// <summary>
        /// Get IP information 得到本地IP地址信息
        /// </summary>
        /// <returns>IP information</returns>
        public IPInfo IPInfo
        {
            get
            {
                lock (m_generalLock)
                {
                    return m_ipInfo;
                }
            }
        }

        /// <summary>
        /// Get managing server 得到管理此客户端的服务器
        /// </summary>
        /// <returns>managing server</returns>
        public INetworkServer Server
        {
            get
            {
                lock (m_generalLock)
                {
                    return m_server;
                }
            }
        }

        /// <summary>
        /// Flag for NoDelay 延时标志
        /// </summary>
        public bool NoDelay
        {
            get
            {
                lock (m_generalLock)
                {
                    return m_noDelay;
                }
            }
            set
            {
                lock (m_generalLock)
                {
                    m_noDelay = value;
                    m_client.NoDelay = m_noDelay;
                }
            }
        }
        /// <summary>
        /// callback obj property 回调属于
        /// </summary>
        public INetworkSocketCallback CallBackObj
        {
            get
            {
                lock (m_generalLock)
                {
                    return m_callBackObj;
                }
            }
            set
            {
                lock (m_generalLock)
                {
                    if (m_callBackObj != null)
                    {
                        m_onNewConnection -= m_callBackObj.OnNewConnection;
                        m_onSent -= m_callBackObj.OnSent;
                        m_onReceived -= m_callBackObj.OnReceived;
                        m_onDisconnect -= m_callBackObj.OnDisconnect;
                    }
                    m_callBackObj = value;
                    if (m_callBackObj != null)
                    {
                        m_onNewConnection += m_callBackObj.OnNewConnection;
                        m_onSent += m_callBackObj.OnSent;
                        m_onReceived += m_callBackObj.OnReceived;
                        m_onDisconnect += m_callBackObj.OnDisconnect;
                    }
                }
            }
        }

        /// <summary>
        /// Start the new connection, and inform the callback object, that the new connection is made
        /// 开始一个新的连接, 并创建个一callback对象, 这个连接并创建
        /// </summary>
        protected override void execute()
        {
            IsConnectionAlive = true;
            startReceive();
            OnNewConnection(this);
        }

        /// <summary>
        /// Disconnect the client socket
        /// 断开此连接
        /// </summary>
        public void Disconnect()
        {
            lock (m_generalLock)
            {
                if (!IsConnectionAlive)
                    return;
                try
                {
                    // tcp client Shutdown函数
                    m_client.Client.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                }
                m_client.Close();
                IsConnectionAlive = false;
            }
            // 从服务器端移除这个客户端
            m_server.DetachClient(this);

            Task t = new Task(delegate ()
            {
                // 回调用户写的回调函数, 让用户处理回调的事情
                OnDisconnect(this);
            });
            t.Start();

        }

        /// <summary>
        /// Check if the connection is alive
        /// 检查当前连接是否正常
        /// </summary>
        /// <returns>true if connection is alive, otherwise false</returns>
        public bool IsConnectionAlive
        {
            get
            {
                lock (m_generalLock)
                {
                    // 这里本没有真实的解析用户的当前连接状态
                    return m_isConnected; 
                }
            }
            private set
            {
                lock (m_generalLock)
                {
                    m_isConnected = value;
                }
            }
        }

        /// <summary>
        /// Send given buf to the client
        /// </summary>
        /// <param name="packet">the packet to send</param>
        public void Send(byte[] buf)
        {
            if (!IsConnectionAlive)
            {
                Task t = new Task(delegate ()
                {
                    // 回调, 告诉用户连接已断开
                    OnSent(this, SendStatus.FAIL_NOT_CONNECTED, buf);
                });
                t.Start();

                return;
            }
            if (buf.Length <= 0)
            {
                Task t = new Task(delegate ()
                {
                    // 回调, 告诉用户发送的数据长度不对
                    OnSent(this, SendStatus.FAIL_INVALID_PACKET, buf);
                });
                t.Start();

                return;
            }

            lock (m_sendLock)
            {
                // 进行正式的发送
                BufTransporter transport = new BufTransporter(buf, 0, buf.Length,this);
                if (m_sendEvent.TryLock())
                {
                    // 进入发送
                    try { m_client.Client.BeginSend(buf, 0, buf.Length, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onSent), transport); }
                    catch (Exception ex)
                    {
                        // 发送出异常,断开连接
                        Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                        Disconnect();
                        return;
                    }
                }
                else
                {
                    lock (m_sendQueueLock)
                    {
                        // 表示压入队列, 等待发送
                        m_sendQueue.Enqueue(transport);
                    }
                }
            }
        }

        /// <summary>
        /// Send given data to the client 供Client使用进行数据发送
        /// </summary>
        /// <param name="data">data in byte array</param>
        /// <param name="offset">offset in bytes</param>
        /// <param name="dataSize">data size in bytes</param>
        public void Send(byte[] data, int offset, int dataSize)
        {
            byte[] send = new byte[dataSize - offset];
            Array.Copy(data, offset, send, 0, dataSize);
            Send(send);
        }




      
        /// <summary>
        /// Buf Transporter class
        /// </summary>
        private class BufTransporter
        {
            /// <summary>
            /// 接收的数据空间, 和发送共用的,,
            /// </summary>
            public byte[] m_data;
            /// <summary>
            /// offset 偏移 发送和接收共用
            /// </summary>
            public int m_offset;
            /// <summary>
            /// size in byte 数据大小 发送和接收共用
            /// </summary>
            public int m_size;
            /// <summary>
            /// client socket 客户端Socket封装的对象
            /// </summary>
            public IocpTcpSocket m_iocpTcpClient;
           
     
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="data">此对象表示接收的数据大小</param>
            /// <param name="offset"></param>
            /// <param name="size"></param>
            /// <param name="iocpTcpClient"></param>
            public BufTransporter(byte[] data, int offset, int size, IocpTcpSocket iocpTcpClient)
            {
                m_data = data;
                m_offset = offset;
                m_size = size;
                m_iocpTcpClient = iocpTcpClient;
            }
        }
        /// <summary>
        /// 开始接收数据
        /// </summary>
        private void startReceive()
        {
            // 开启接收
            BufTransporter transport = new BufTransporter(m_recvData, 0, m_recvData.Length, this);
            try { m_client.Client.BeginReceive(transport.m_data, 0, transport.m_data.Length, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onReceived), transport); }
            catch (Exception ex)
            {
                // 异步接收出现问题, 就断开连接
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                Disconnect();
                return;
            }

        }

        /// <summary>
        /// Receive callback function 接收异步函数
        /// </summary>
        /// <param name="result">result</param>
        private static void onReceived(IAsyncResult result)
        {
            // BufTransporter 一个异步对像
            BufTransporter transport = result.AsyncState as BufTransporter;
            Socket socket = transport.m_iocpTcpClient.m_client.Client;

            int readSize = 0;
            try
            {
                if (socket != null)
                    readSize = socket.EndReceive(result);
            }
            catch (Exception ex)
            {
                // 读取出现异常,则断开连接
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                transport.m_iocpTcpClient.Disconnect();
                return;
            }
            if (readSize == 0)
            {
                // 读取到0也是异常: 这个可能的原因就是网络客户端被客户端断开
                transport.m_iocpTcpClient.Disconnect();
                return;
            }
            // 读取客户端的数据
            byte[] recvdata = new byte[readSize];
            // 复制客户端的数据
            Array.Copy(transport.m_data, 0, recvdata, 0, readSize);
            // 回调给客户端
            transport.m_iocpTcpClient.OnReceived(transport.m_iocpTcpClient, recvdata);
            // 启用下一个接收
            try { socket.BeginReceive(transport.m_data, 0, transport.m_data.Length, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onReceived), transport); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                transport.m_iocpTcpClient.Disconnect();
                return;
            }
            
        }

        /// <summary>
        /// Send callback function 发送异步函数
        /// </summary>
        /// <param name="result">result</param>
        private static void onSent(IAsyncResult result)
        {
            BufTransporter transport = result.AsyncState as BufTransporter;
            Socket socket = transport.m_iocpTcpClient.m_client.Client;

            int sentSize = 0;
            // 发送完成
            try { sentSize = socket.EndSend(result); }
            catch (Exception ex)
            {
                //发送出现错误,断开连接
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                transport.m_iocpTcpClient.Disconnect();
                // 回调给客户端, 并把发送的数据给客户端
                transport.m_iocpTcpClient.OnSent(transport.m_iocpTcpClient, SendStatus.FAIL_SOCKET_ERROR, transport.m_data);
                return;
            }
            if (sentSize == 0)
            {
                // 发送的字节为0, 也是异常
                transport.m_iocpTcpClient.Disconnect();
                // 表示客户已经断开连接
                transport.m_iocpTcpClient.OnSent(transport.m_iocpTcpClient, SendStatus.FAIL_CONNECTION_CLOSING, transport.m_data);
                return;
            }
            if (sentSize < transport.m_size)
            {
                // 发送的size小于要发送的size,则接着发送
                transport.m_offset = transport.m_offset + sentSize;
                transport.m_size = transport.m_size - sentSize;
                try { socket.BeginSend(transport.m_data, transport.m_offset, transport.m_size, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onSent), transport); }
                catch (Exception ex)
                {
                    // BeginSend发送出现问题,则断开连接
                    Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                    transport.m_iocpTcpClient.Disconnect();
                    transport.m_iocpTcpClient.OnSent(transport.m_iocpTcpClient, SendStatus.FAIL_SOCKET_ERROR, transport.m_data);
                    return;
                }
            }
            else
            {
                // 发送成功, 则检查有没需要继续发送的数据
                BufTransporter delayedTransport = null;
                lock (transport.m_iocpTcpClient.m_sendQueueLock)
                {
                    Queue<BufTransporter> sendQueue = transport.m_iocpTcpClient.m_sendQueue;
                    if (sendQueue.Count > 0)
                    {
                        delayedTransport = sendQueue.Dequeue();
                    }
                }
                if (delayedTransport != null)
                {
                    // 有则发送
                    try { socket.BeginSend(delayedTransport.m_data, 0, delayedTransport.m_data.Length, SocketFlags.None, new AsyncCallback(IocpTcpSocket.onSent), delayedTransport); }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                        delayedTransport.m_iocpTcpClient.Disconnect();
                        delayedTransport.m_iocpTcpClient.OnSent(delayedTransport.m_iocpTcpClient, SendStatus.FAIL_SOCKET_ERROR, delayedTransport.m_data);
                        return;
                    }
                }
                else
                {
                    // 没有则释放锁
                    transport.m_iocpTcpClient.m_sendEvent.Unlock();
                }
                // 通知用户发送的数据成功
                transport.m_iocpTcpClient.OnSent(transport.m_iocpTcpClient, SendStatus.SUCCESS, transport.m_data);
            }

        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        ///  <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Default initialization for a bool is 'false'</remarks>
        private bool IsDisposed { get; set; }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        private void Dispose(bool isDisposing)
        {
            try
            {
                if (!this.IsDisposed)
                {
                    if (IsConnectionAlive)
                        Disconnect();
                    if (isDisposing)
                    {
                        // Free any other managed objects here.
                        if (m_sendEvent != null)
                        {
                            m_sendEvent.Dispose();
                            m_sendEvent = null;
                        }
                    }
                }
            }
            finally
            {
                this.IsDisposed = true;
            }
        }
    }
}
