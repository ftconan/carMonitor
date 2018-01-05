using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RfidNet
{
    public class ClientMsg
    {
        private List<Byte> mBuf = new List<byte>();
        private List<Byte> mWriteReturnCommand = new List<byte>();
        private List<Byte> mReadReturnCommand = new List<byte>();
        private ConnectionMsg mMsg = new ConnectionMsg();
        private DateTime mCreateTime;
        private DateTime mLastTime; 
         
        public ClientMsg(INetworkSocket socket)
        {
            mCreateTime = DateTime.Now;
            mMsg.Ip = socket.IPInfo.IPAddress;
            mMsg.Port = socket.IPInfo.IPEndPoint.Port;
        }

        public List<Byte> GetBuf()
        {
            return mBuf;
        }

        public ConnectionMsg GetConnectionMsg()
        {
            return mMsg;
        }

        public void SetDeviceId(int nDeviceId)
        {
            mLastTime = DateTime.Now;
            mMsg.DeviceId = nDeviceId;
        }

        public int GetDeviceId()
        {
            return mMsg.DeviceId;
        }

        public void SetWriteReturnCommand(byte[] data)
        {
            mWriteReturnCommand.AddRange(data);
        }

        public byte[] GetWriteReturnCommand()
        {
            return mWriteReturnCommand.ToArray();
        }

        public void SetReadReturnCommand(byte[] data)
        {
            mReadReturnCommand.AddRange(data);
        }

        public byte[] GetReadReturnCommand()
        {
            return mReadReturnCommand.ToArray();
        }
    }
}
