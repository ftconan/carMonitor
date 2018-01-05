using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;


namespace RfidNet
{ 
    public sealed class IocpTcpServer :ThreadEx, INetworkServer
    {
        /// <summary>
        /// 端口
        /// </summary>
        private String m_port = ServerConf.DEFAULT_PORT;

        /// <summary>
        /// NoDelay 标志
        /// </summary>
        private bool m_noDelay = true;

        /// <summary>
        /// maximum socket 最大连接数
        /// </summary>
        private int m_maxSocketCount = SocketCount.Infinite;
        /// <summary>
        /// listner 对象
        /// </summary>
        private TcpListener m_listener = null;
        /// <summary>
        /// server option 服务器端选项
        /// </summary>
        private ServerOps m_serverOps = null;

        /// <summary>
        /// callback object 服务端的回调
        /// </summary>
        private INetworkServerCallback m_callBackObj = null;

        /// <summary>
        /// acceptor object accetor 对象
        /// </summary>
        private INetworkServerAcceptor m_acceptor = null;

        /// <summary>
        /// general lock 通用锁
        /// </summary>
        private Object m_generalLock = new Object();

        /// <summary>
        /// client socket list lock 客户端连接对象
        /// </summary>
        private Object m_listLock = new Object();

      
        /// <summary>
        /// client socket list 客户端List管理对象
        /// </summary>
        private HashSet<IocpTcpSocket> m_socketList = new HashSet<IocpTcpSocket>();

      
        /// <summary>
        /// OnServerStarted event
        /// 服务器启动事件
        /// </summary>
        OnServerStartedDelegate m_onServerStarted = delegate { };
        /// <summary>
        ///  OnAccept event
        ///  服务器Accept对象
        /// </summary>
        OnServerAcceptedDelegate m_onAccepted = delegate { };
        /// <summary>
        /// OnserverStopped event
        /// </summary>
        OnServerStoppedDelegate m_onServerStopped = delegate { };

        /// <summary>
        /// OnServerStarted event
        /// 接口 
        /// </summary>
        public OnServerStartedDelegate OnServerStarted
        {
            get
            {
                return m_onServerStarted;
            }
            set
            {
                if (value == null)
                {
                    m_onServerStarted = delegate { };
                    if (CallBackObj != null)
                        m_onServerStarted += CallBackObj.OnServerStarted;
                }
                else
                {
                    m_onServerStarted = CallBackObj != null && CallBackObj.OnServerStarted != value ? CallBackObj.OnServerStarted + (value - CallBackObj.OnServerStarted) : value;
                }
            }
        }
        /// <summary>
        ///  OnAccept event
        ///  Accept 接口
        /// </summary>
        public OnServerAcceptedDelegate OnServerAccepted
        {
            get
            {
                return m_onAccepted;
            }
            set
            {
                if (value == null)
                {
                    m_onAccepted = delegate { };
                    if (CallBackObj != null)
                        m_onAccepted += CallBackObj.OnServerAccepted;
                }
                else
                {
                    m_onAccepted = CallBackObj != null && CallBackObj.OnServerAccepted != value ? CallBackObj.OnServerAccepted + (value - CallBackObj.OnServerAccepted) : value;
                }
            }
        }
        /// <summary>
        /// OnserverStopped event
        /// Stop 接口
        /// </summary>
        public OnServerStoppedDelegate OnServerStopped
        {
            get
            {
                return m_onServerStopped;
            }
            set
            {
                if (value == null)
                {
                    m_onServerStopped = delegate { };
                    if (CallBackObj != null)
                        m_onServerStarted += CallBackObj.OnServerStarted;
                }
                else
                {
                    m_onServerStopped = CallBackObj != null && CallBackObj.OnServerStopped != value ? CallBackObj.OnServerStopped + (value - CallBackObj.OnServerStopped) : value;
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// 默认构造
        /// </summary>
        public IocpTcpServer()
            : base()
        {
        }

        /// <summary>
        /// Default copy constructor
        /// 默认构造复制构造
        /// </summary>
        /// <param name="b">the object to copy from</param>
        public IocpTcpServer(IocpTcpServer b)
            : base(b)
        {
            m_port = b.m_port;
            m_serverOps = b.m_serverOps;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~IocpTcpServer()
        {
            if (IsServerStarted)
                StopServer();
        }

        /// <summary>
        /// Return port 
        /// 端口set
        /// </summary>
        /// <returns>port</returns>
        public String Port
        {
            get
            {
                lock (m_generalLock)
                {
                    return m_port;
                }

            }
            private set
            {
                lock (m_generalLock)
                {
                    m_port = value;
                }
            }
        }
        /// <summary>
        /// acceptor object
        /// Accept对象
        /// </summary>
        public INetworkServerAcceptor Acceptor
        {
            get
            {
                lock (m_generalLock)
                {
                    return m_acceptor;
                }
            }
            set
            {
                lock (m_generalLock)
                {
                    if (value == null)
                        throw new NullReferenceException("Acceptor cannot be null!");
                    m_acceptor = value;
                }

            }
        }
        /// <summary>
        /// callback object
        /// 服务器监听对象回调
        /// </summary>
        public INetworkServerCallback CallBackObj
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
                        m_onServerStarted -= m_callBackObj.OnServerStarted;
                        m_onServerStopped -= m_callBackObj.OnServerStopped;
                    }
                    m_callBackObj = value;
                    if (m_callBackObj != null)
                    {
                        m_onServerStarted += m_callBackObj.OnServerStarted;
                        m_onServerStopped += m_callBackObj.OnServerStopped;
                    }
                }
            }
        }

      
        /// <summary>
        /// No delay property
        /// Delay状态
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
                }
            }
        }

        /// <summary>
        /// 最大连接属性
        /// maximum socket count property
        /// </summary>
        public int MaxSocketCount
        {
            get
            {
                lock (m_generalLock)
                {
                    return m_maxSocketCount;
                }
            }
            set
            {
                lock (m_generalLock)
                {
                    m_maxSocketCount = value;
                }
            }
        }
        /// <summary>
        /// Callback Exception class
        /// 回调异常类
        /// </summary>
        [Serializable]
        private class CallbackException : Exception
        {
            /// <summary>
            /// Default constructor
            /// 构造函数
            /// </summary>
            public CallbackException()
                : base()
            {

            }

            /// <summary>
            /// Default constructor
            /// 构造函数
            /// </summary>
            /// <param name="message">message for exception</param>
            public CallbackException(String message)
                : base(message)
            {

            }
        }

        /// <summary>
        /// Start the server and start accepting the client
        /// 开启server 并启动接收客户端连接
        /// </summary>
        protected override void execute()
        {
            // 初始状态
            StartStatus status = StartStatus.FAIL_SOCKET_ERROR;
            try
            {
                lock (m_generalLock)
                {
                    // 判断是否已经启动
                    if (IsServerStarted)
                    {
                        status = StartStatus.FAIL_ALREADY_STARTED;
                        throw new CallbackException();
                    }
                    // 初始化赋值
                    Acceptor = m_serverOps.Acceptor;
                    CallBackObj = m_serverOps.CallBackObj;
                    NoDelay = m_serverOps.NoDelay;
                    Port = m_serverOps.Port;
                    MaxSocketCount = m_serverOps.MaxSocketCount;

                    if (Port == null || Port.Length == 0)
                    {
                        Port = ServerConf.DEFAULT_PORT;
                    }
                    lock (m_listLock)
                    {
                        // 清除
                        m_socketList.Clear();
                    }
                    // 创建监听socket
                    m_listener = new TcpListener(IPAddress.Any, Convert.ToInt32(m_port));
                    m_listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    m_listener.Start();
                    m_listener.BeginAcceptTcpClient(new AsyncCallback(IocpTcpServer.onAccept), this);
                }

            }
            catch (CallbackException)
            {
                // 失败则执行这里
                OnServerStarted(this, status);
                return;
            }
            catch (Exception ex)
            {
                // 其它异常,如端口已经常不可,IP地址不可用
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                if (m_listener != null)
                    m_listener.Stop();
                m_listener = null;
                OnServerStarted(this, StartStatus.FAIL_SOCKET_ERROR);
                return;
            }
            // 成功则执行这里
            OnServerStarted(this, StartStatus.SUCCESS);
        }

        /// <summary>
        /// Accept callback function
        /// </summary>
        /// <param name="result">result</param>
        private static void onAccept(IAsyncResult result)
        {
            IocpTcpServer server = result.AsyncState as IocpTcpServer;
            TcpClient client = null;
            try
            {
                if (server.m_listener != null)
                {
                    client = server.m_listener.EndAcceptTcpClient(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                if (client != null)
                {
                    try
                    {
                        // 异常则关闭
                        client.Client.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + " >" + e.StackTrace);
                    }
                    client.Close();
                    client = null;
                }
            }

            try
            {
                if (server.m_listener != null)
                    // 开始接收 调用IocpTcpServer的OnAccept
                    server.m_listener.BeginAcceptTcpClient(new AsyncCallback(IocpTcpServer.onAccept), server);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " >" + ex.StackTrace);
                if (client != null)
                    client.Close();
                server.StopServer();
                return;
            }

            if (client != null)
            {
                IocpTcpSocket socket = new IocpTcpSocket(client, server);
                lock (server.m_listLock)
                {
                    if (server.m_socketList.Count > server.MaxSocketCount)
                    //if (server.MaxSocketCount != SocketCount.Infinite && server.m_socketList.Count > server.MaxSocketCount)
                    {
                        socket.Disconnect();
                        return;
                    }
                }
                if (server.CallBackObj == null)
                {
                    socket.Disconnect();
                    return;
                }

                if (!server.Acceptor.OnAccept(server, socket.IPInfo))
                {
                    socket.Disconnect();
                }
                else
                {
                    INetworkSocketCallback socketCallbackObj = server.Acceptor.GetSocketCallback();
                    socket.CallBackObj = socketCallbackObj;
                    socket.Start();
                    lock (server.m_listLock)
                    {
                        server.m_socketList.Add(socket);
                    }
                    server.OnServerAccepted(server, socket);
                }
            }


        }

        /// <summary>
        /// Start the server with given option
        /// </summary>
        /// <param name="ops">options</param>
        public void StartServer(ServerOps ops)
        {
            if (ops == null)
                ops = ServerOps.defaultServerOps;
            if (ops.Acceptor == null)
                throw new NullReferenceException("acceptor cannot be null!");
            lock (m_generalLock)
            {
                m_serverOps = ops;
            }
            Start();
        }
        /// <summary>
        /// Stop the server
        /// </summary>
        public void StopServer()
        {
            lock (m_generalLock)
            {
                if (!IsServerStarted)
                    return;

                m_listener.Stop();
                m_listener = null;
            }
            ShutdownAllClient();

            OnServerStopped(this);
        }

        /// <summary>
        /// Check if the server is started
        /// </summary>
        /// <returns>true if the server is started, otherwise false</returns>
        public bool IsServerStarted
        {
            get
            {
                lock (m_generalLock)
                {
                    if (m_listener != null)
                        return true;
                    return false;
                }
            }
        }
        /// <summary>
        /// Shut down all the client, connected
        /// </summary>
        public void ShutdownAllClient()
        {
            lock (m_listLock)
            {
                List<IocpTcpSocket> socketList = GetClientSocketList();
                foreach (IocpTcpSocket socket in socketList)
                {
                    socket.Disconnect();
                }
            }
        }      
 
        /// <summary>
        /// Return the client socket list
        /// </summary>
        /// <returns>the client socket list</returns>
        public List<IocpTcpSocket> GetClientSocketList()
        {
            lock (m_listLock)
            {
                return new List<IocpTcpSocket>(m_socketList);
            }
        }

        /// <summary>
        /// Detach the given client from the server management
        /// </summary>
        /// <param name="clientSocket">the client to detach</param>
        /// <returns></returns>
        public bool DetachClient(IocpTcpSocket clientSocket)
        {
            lock (m_listLock)
            {
                return m_socketList.Remove(clientSocket);
            }
        }
    }
}
