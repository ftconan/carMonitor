using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RfidNet
{
    /// <summary>
    /// Connect status
    /// </summary>
    public enum ConnectStatus
    {
        /// <summary>
        /// Success
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// Failed by time-out
        /// </summary>
        FAIL_TIME_OUT,
        /// <summary>
        /// Failed due to connection already exists
        /// </summary>
        FAIL_ALREADY_CONNECTED,
        /// <summary>
        /// Failed due to unknown error
        /// </summary>
        FAIL_SOCKET_ERROR
    }

    /// <summary>
    /// Server start status
    /// </summary>
    public enum StartStatus
    {
        /// <summary>
        /// Success
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// Failed due to server already started
        /// </summary>
        FAIL_ALREADY_STARTED,
        /// <summary>
        /// Failed due to socket error
        /// </summary>
        FAIL_SOCKET_ERROR
    }

    /// <summary>
    /// Send status
    /// </summary>
    public enum SendStatus : uint
    {
        /// <summary>
        /// Success
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// Failed due to socket error
        /// </summary>
        FAIL_SOCKET_ERROR,
        /// <summary>
        /// Failed due to no connection exists
        /// </summary>
        FAIL_NOT_CONNECTED,
        /// <summary>
        /// Failed due to invalid packet
        /// </summary>
        FAIL_INVALID_PACKET,
        /// <summary>
        /// Failed due to connection closing
        /// </summary>
        FAIL_CONNECTION_CLOSING,

    };
    /// <summary>
    /// Server configuration class
    /// </summary>
    public class ServerConf
    {
        /// <summary>
        /// Default hostname (localhost)
        /// </summary>
	    public const String DEFAULT_HOSTNAME = "localhost";
        /// <summary>
        /// Default port (80808)
        /// </summary>
        public const String DEFAULT_PORT = "32500";
    }

    public static class SocketCount
    {
        // 最大10个连接
        public const int Infinite = 10;
    }

}
