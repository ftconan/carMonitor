
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace RfidNet
{
    /// <summary>
    /// Server option class
    /// </summary>
    public sealed class ServerOps
    {
        /// <summary>
        /// acceptor object 接收器
        /// </summary>
        public INetworkServerAcceptor Acceptor
        {
            get;
            set;
        }

        /// <summary>
        /// callback object 回调对象
        /// </summary>
        public INetworkServerCallback CallBackObj
        {
            get;
            set;
        }


       
        /// <summary>
        /// port 端口
        /// </summary>
        public String Port
        {
            get;
            set;
        }
        /// <summary>
        /// IP
        /// </summary>
        public String Host
        {
            get;
            set;
        }
        /// <summary>
        /// flag for no delay 延时标志
        /// </summary>
        public bool NoDelay
        {
            get;
            set;
        }
        /// <summary>
        ///  最大连接数
        /// </summary>
        public int MaxSocketCount
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor 模认构造函数
        /// </summary>
        public ServerOps()
        {
            Acceptor = null;
            CallBackObj = null;
            Host = ServerConf.DEFAULT_HOSTNAME;
            Port = ServerConf.DEFAULT_PORT;
            NoDelay = true;
            MaxSocketCount = SocketCount.Infinite;

        }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="acceptor">acceptor object</param>
        /// <param name="callBackObj">callback object</param>
        /// <param name="port">port</param>
        /// <param name="noDelay">noDelay falg</param>
        public ServerOps(INetworkServerAcceptor acceptor, String host, String port, INetworkServerCallback callBackObj = null, bool noDelay = true)
        {
            this.Host = host;
            this.Port = port;
            this.Acceptor = acceptor;
            this.CallBackObj = callBackObj;
            this.NoDelay = noDelay;
            this.MaxSocketCount = 100;
        }

        /// <summary>
        /// Default server option 默认Server选项
        /// </summary>
        public static ServerOps defaultServerOps = new ServerOps();
    };

    /// <summary>
    /// Server interface 服务器端接口
    /// </summary>
    public interface INetworkServer
    {
        /// <summary>
        /// Return the port 返回服务器监听端口
        /// </summary>
        /// <returns>port</returns>
        String Port { get; }

        /// <summary>
        /// acceptor object 服务器接受对象
        /// </summary>
        INetworkServerAcceptor Acceptor
        {
            get;
            set;
        }
        /// <summary>
        /// callback object 回调对象
        /// </summary>
        INetworkServerCallback CallBackObj
        {
            get;
            set;
        }
 

        /// <summary>
        /// flag for no delay 是否延时标志
        /// </summary>
        bool NoDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Start the server with given option 启动服务器,对过给定服务器的参数
        /// </summary>
        /// <param name="ops">option for the server</param>
        void StartServer(ServerOps ops);

        /// <summary>
        /// Stop the server 停止服务器
        /// </summary>
        void StopServer();

        /// <summary>
        /// Check whether server is started or not 检查当前服务器的启动状态
        /// </summary>
        /// <returns>true if server is started, otherwise false</returns>
        bool IsServerStarted { get; }
        /// <summary>
        /// Shutdown all the client, connected 关闭所有的对象
        /// </summary>
        void ShutdownAllClient();
        /// <summary>
        /// Return the connected client list 返回客户端连链表
        /// </summary>
        /// <returns>the connected client list</returns>
        List<IocpTcpSocket> GetClientSocketList();

        /// <summary>
        /// Detach the given client from the server management, 移除给定的客户端
        /// </summary>
        /// <param name="clientSocket">the client to detach</param>
        /// <returns>true if successful, otherwise false</returns>
        bool DetachClient(IocpTcpSocket clientSocket);


        /// <summary>
        /// OnServerStarted event 服务器启动事件
        /// </summary>
        OnServerStartedDelegate OnServerStarted
        {
            get;
            set;
        }
        /// <summary>
        ///  OnAccept event 服务器启动接收事件
        /// </summary>
        OnServerAcceptedDelegate OnServerAccepted
        {
            get;
            set;
        }
        /// <summary>
        /// OnserverStopped event  服务器停止事件
        /// </summary>
        OnServerStoppedDelegate OnServerStopped
        {
            get;
            set;
        }

    }

    public delegate void OnServerStartedDelegate(INetworkServer server, StartStatus status);
    public delegate void OnServerAcceptedDelegate(INetworkServer server, INetworkSocket socket);
    public delegate void OnServerStoppedDelegate(INetworkServer server);

    /// <summary>
    /// Server callback interface 服务器回调接口
    /// </summary>
    public interface INetworkServerCallback
    {
        /// <summary>
        /// Server started callback 服务器启动回调接口
        /// </summary>
        /// <param name="server">server</param>
        /// <param name="status">start status</param>
        void OnServerStarted(INetworkServer server, StartStatus status);
        /// <summary>
        /// Accept callback Accept回调接口
        /// </summary>
        /// <param name="server">server</param>
        /// <param name="socket">socket accepted</param>
        void OnServerAccepted(INetworkServer server, INetworkSocket socket);
        /// <summary>
        /// Server stopped callback 停止回调接口
        /// </summary>
        /// <param name="server">server</param>
        void OnServerStopped(INetworkServer server);
    };

    /// <summary>
    /// Server acceptor interface 服务器Acceptor回调接口
    /// </summary>
    public interface INetworkServerAcceptor
    {
        /// <summary>
        /// Accept callback Accept回调
        /// </summary>
        /// <param name="server">server</param>
        /// <param name="ipInfo">connection info</param>
        /// <returns>the socket callback interface</returns>
        bool OnAccept(INetworkServer server, IPInfo ipInfo);
        /// <summary>
        /// Should return the socket callback object 得Socket callback object对象
        /// </summary>
        /// <returns>the socket callback object</returns>
        INetworkSocketCallback GetSocketCallback();
    };

    /// <summary>
    /// Socket interface
    /// </summary>
    public interface INetworkSocket
    {
        /// <summary>
        /// Disconnect the client Client断开
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Check if the connection is alive 检查当前连接是否正常
        /// </summary>
        /// <returns>true if the connection is alive, otherwise false</returns>
        bool IsConnectionAlive { get; }


        /// <summary>
        /// Send given data to the client 发送数据
        /// </summary>
        /// <param name="data">data in byte array</param>
        /// <param name="offset">offset in bytes</param>
        /// <param name="dataSize">data size in bytes</param>
        void Send(byte[] data, int offset, int dataSize);

        /// <summary>
        /// Send given data to the client, 发送数据
        /// </summary>
        /// <param name="data">data in byte array</param>
        void Send(byte[] data);

        /// <summary>
        /// Return the IP information of the client 返回这个客户端的IP信息
        /// </summary>
        /// <returns>the IP information of the client</returns>
        IPInfo IPInfo { get; }

        /// <summary>
        /// Return the server managing this socket,返回管理这个客户端的Socket信息
        /// </summary>
        /// <returns>the server managing this socket</returns>
        INetworkServer Server { get; }

        /// <summary>
        /// No delay flag 客户端delay标志
        /// </summary>
        bool NoDelay { get; set; }

        /// <summary>
        /// callback object 客户端回调对象
        /// </summary>
        INetworkSocketCallback CallBackObj
        {
            get;
            set;
        }


        /// <summary>
        /// OnNewConnected event, 新的连接事件
        /// </summary>
        OnSocketNewConnectionDelegate OnNewConnection
        {
            get;
            set;
        }
        /// <summary>
        /// OnRecevied event, 数据来到事件
        /// </summary>
        OnSocketReceivedDelegate OnReceived
        {
            get;
            set;
        }
        /// <summary>
        /// OnSent event 发送事件
        /// </summary>
        OnSocketSentDelegate OnSent
        {
            get;
            set;
        }
        /// <summary>
        /// OnDisconnect event 断开事件
        /// </summary>
        OnSocketDisconnectDelegate OnDisconnect
        {
            get;
            set;
        }
    }


    public delegate void OnSocketNewConnectionDelegate(INetworkSocket socket);
    public delegate void OnSocketReceivedDelegate(INetworkSocket socket, byte[] recvBuf);
    public delegate void OnSocketSentDelegate(INetworkSocket socket, SendStatus status, byte[] sendBuf);
    public delegate void OnSocketDisconnectDelegate(INetworkSocket socket);


    /// <summary>
    /// Socket callback interface
    /// </summary>
    public interface INetworkSocketCallback
    {
        /// <summary>
        /// NewConnection callback 新连接到来回调事件
        /// </summary>
        /// <param name="socket">client socket</param>
        void OnNewConnection(INetworkSocket socket);

        /// <summary>
        /// Receive callback 数据来到回调事件
        /// </summary>
        /// <param name="socket">client socket</param>
        /// <param name="receivedPacket">received packet</param>
        void OnReceived(INetworkSocket socket, byte[] recvBuf);

        /// <summary>
        /// Send callback 发送完成回调事件
        /// </summary>
        /// <param name="socket">client socket</param>
        /// <param name="status">stend status</param>
        /// <param name="sentPacket">sent packet</param>
        void OnSent(INetworkSocket socket, SendStatus status, byte[] sendBuf);

        /// <summary>
        /// Disconnect callback 断开回调事件
        /// </summary>
        /// <param name="socket">client socket</param>
        void OnDisconnect(INetworkSocket socket);
    };

    /// <summary>
    /// IP End-point type IP地址类型,本地IP,远程IP
    /// </summary>
    public enum IPEndPointType
    {
        /// <summary>
        /// local
        /// </summary>
        LOCAL = 0,
        /// <summary>
        /// remote
        /// </summary>
        REMOTE
    }

    /// <summary>
    /// IP Information class IP地址信息
    /// </summary>
    public sealed class IPInfo
    {
        /// <summary>
        /// IP Address string IP 地址String
        /// </summary>
        String m_ipAddress;
        /// <summary>
        /// IP End-Point IP-end
        /// </summary>
        IPEndPoint m_ipEndPoint;
        /// <summary>
        /// IP End-Point type IP地址类型
        /// </summary>
        IPEndPointType m_ipEndPointType;

        /// <summary>
        /// Default constructor 默认构造函数
        /// </summary>
        /// <param name="ipAddress">IP Address string</param>
        /// <param name="ipEndPoint">IP End-Point</param>
        /// <param name="ipEndPointType">IP End-Point type</param>
        public IPInfo(String ipAddress, IPEndPoint ipEndPoint, IPEndPointType ipEndPointType)
        {
            m_ipAddress = ipAddress;
            m_ipEndPoint = ipEndPoint;
            m_ipEndPointType = ipEndPointType;
        }
        /// <summary>
        /// Return the IP address string
        /// </summary>
        /// <returns>the IP address string</returns>
        public String IPAddress
        {
            get
            {
                return m_ipAddress;
            }
        }

        /// <summary>
        /// Return the IP End-point
        /// </summary>
        /// <returns>the IP End-point</returns>
        public IPEndPoint IPEndPoint
        {
            get
            {
                return m_ipEndPoint;
            }
        }

        /// <summary>
        /// Return the IP End-point type
        /// </summary>
        /// <returns>the IP End-point type</returns>
        public IPEndPointType IPEndPointType
        {
            get
            {
                return m_ipEndPointType;
            }
        }
    }
}
