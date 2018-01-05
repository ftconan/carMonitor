using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RfidNet
{
    public  sealed class RfidServer : INetworkServerAcceptor, INetworkServerCallback, INetworkSocketCallback
    {
        public RfidServer()
        {
            this.mMaxTempMsgCount = 1024;
            this.mMaxConnectionCount = 100;
        }
        private int mPort;
        private int mMaxTempMsgCount;
        private List<TagMsg> mMsg = new List<TagMsg>();
        private int mMaxConnectionCount;
        private INetworkServer mServer = new IocpTcpServer();
        private Dictionary<INetworkSocket, ClientMsg> mSocketList = new Dictionary<INetworkSocket, ClientMsg>();

        public string GetVersion()
        {

            // "1.0.1"  2017 08 21 生成
            return "1.0.1";
        }

        public bool StartServer(int nPort, int nMaxTempMsgCount)
        {
            this.mPort = nPort;
            this.mMaxTempMsgCount = nMaxTempMsgCount;

            ServerOps ops = new ServerOps(this, "localhost", mPort.ToString(), this);
            mServer.StartServer(ops);
            Thread.Sleep(1000);
            return mServer.IsServerStarted;
        }
        public bool StopServer()
        {
            if (mServer.IsServerStarted)
                mServer.StopServer();
            Thread.Sleep(400);
            mMsg.Clear();
            mSocketList.Clear();
            return mServer.IsServerStarted;
        }

        public bool GetState()
        {
            return mServer.IsServerStarted;
        }
        public int GetMsgCount()
        {
            return mMsg.Count;
        }
        public int GetMaxMsgSize()
        {
            return mMaxTempMsgCount;
        }
        public int SetMaxMsgSize(int nSize)
        {
            return mMaxTempMsgCount = nSize;
        }
        public TagMsg GetMsg()
        {
            if (mMsg.Count == 0)
                return null;
            TagMsg msg = mMsg[0];
            mMsg.RemoveAt(0);
            return msg;
        }
        public int SetMaxConnectionSize(int nSize)
        {
            if (nSize >= 200 || nSize <= 1)
                // 设置的参数超出范围
                return -1;
            return mMaxConnectionCount = nSize;
        }
        public int GetMaxConnectionSize()
        {
            return mMaxConnectionCount;
        }

        public List<ConnectionMsg> GetConnectionMsg()
        {
            List<ConnectionMsg> connectionList = new List<ConnectionMsg>();
            foreach (var item in mSocketList)
            {
                connectionList.Add(item.Value.GetConnectionMsg());
            }

            return connectionList;
        }
  

        public void ReadSetting(int nDeviceId)
        {
            byte[] buf = new byte[9];
            buf[0] = 0x55;
            buf[1] = 0xAA;
            buf[2] = 0x07;
            buf[3] = 0x00;
            buf[5] =(byte)((nDeviceId >> 8) & 0x000000FF);
            buf[4] = (byte)(nDeviceId & 0x000000FF);
            buf[6] = 0x0C;
            buf[7] = 0x00;
            int nCheck = 0;
            for (int i = 0; i < buf.Length - 1; i++)
            {
                nCheck += buf[i];
            }
            buf[8] = (byte)nCheck;
            Write(nDeviceId, buf);
        }

        public SettingMsg GetReadSetting(int nDeviceId)
        {
            foreach (var item in mSocketList)
            {
                if (item.Value.GetDeviceId() == nDeviceId)
                {
                    byte[] data = item.Value.GetReadReturnCommand();
                    if (data.Length == 0)
                    {
                        return null;
                    }
                    else
                    {
                        # region ""
                        SettingMsg msg = new SettingMsg();
                        msg.FValue = data[8];
                        byte local1 = data[9];
                        byte local2 = data[10];
                        byte local3 = data[11];
                        byte local4 = data[12];
                        byte gateway1 = data[15];
                        byte gateway2 = data[16];
                        byte gateway3 = data[17];
                        byte gateway4 = data[18];
                        byte mac1 = data[19];
                        byte mac2 = data[20];
                        byte mac3 = data[21];
                        byte mac4 = data[22];
                        byte mac5 = data[23];
                        byte mac6 = data[24];
                        byte subnet1 = data[25];
                        byte subnet2 = data[26];
                        byte subnet3 = data[27];
                        byte subnet4 = data[28];
                        byte remote1 = data[29];
                        byte remote2 = data[30];
                        byte remote3 = data[31];
                        byte remote4 = data[32];
                        msg.LocalIP = local1.ToString() + "." + local2.ToString() + "." + local3.ToString() + "." + local4.ToString();
                        msg.GateWay = gateway1.ToString() + "." + gateway2.ToString() + "." + gateway3.ToString() + "." + gateway4.ToString();
                        msg.SubNet = subnet1.ToString() + "." + subnet2.ToString() + "." + subnet3.ToString() + "." + subnet4.ToString();
                        msg.MacAddress = mac1.ToString("X2") + "-" +
                            mac2.ToString("X2") + "-" +
                            mac3.ToString("X2") + "-" +
                            mac4.ToString("X2") + "-" +
                            mac5.ToString("X2") + "-" +
                            mac6.ToString("X2");
                        msg.ServerIP = remote1.ToString() + "." +
                             remote2.ToString() + "." +
                             remote3.ToString() + "." +
                             remote4.ToString();
                        msg.DeviceId = (short)(data[4] << 8 | data[5]);
                        msg.LocalPort = (short)(data[14] << 8 | data[13]);
                        msg.ServerPort = (short)(data[34] << 8 | data[33]);
                        return msg;
                        #endregion
                    }
                }
            }
            return null;
        }

        public void WriteSetting(int nDeviceId, SettingMsg msg)
        {
            byte[] buf = new byte[39];
            buf[0] = 0x55;
            buf[1] = 0xAA;
            buf[2] = 0x25;
            buf[3] = 0x00;
            buf[5] = (byte)((nDeviceId >> 8) & 0x000000FF);
            buf[4] = (byte)(nDeviceId & 0x000000FF);
            buf[6] = 0x0B;
            buf[7] = 0x00;
            // 增加 1-95 8 -60

            buf[8] = Convert.ToByte(msg.FValue);
            string[] localips = msg.LocalIP.Split('.');
            buf[9] = Convert.ToByte(localips[0]);
            buf[10] = Convert.ToByte(localips[1]);
            buf[11] = Convert.ToByte(localips[2]);
            buf[12] = Convert.ToByte(localips[3]);
            buf[14] = (byte)((msg.LocalPort & 0xFF00) >> 8);
            buf[13] = (byte)(msg.LocalPort & 0x00FF);
            string[] gateways = msg.GateWay.Split('.');
            buf[15] = Convert.ToByte(gateways[0]);
            buf[16] = Convert.ToByte(gateways[1]);
            buf[17] = Convert.ToByte(gateways[2]);
            buf[18] = Convert.ToByte(gateways[3]);
            string[] macs = msg.MacAddress.Split('-');
            buf[19] = Convert.ToByte(macs[0], 16);
            buf[20] = Convert.ToByte(macs[1], 16);
            buf[21] = Convert.ToByte(macs[2], 16);
            buf[22] = Convert.ToByte(macs[3], 16);
            buf[23] = Convert.ToByte(macs[4], 16);
            buf[24] = Convert.ToByte(macs[5], 16);
            string[] subnets = msg.SubNet.Split('.');
            buf[25] = Convert.ToByte(subnets[0]);
            buf[26] = Convert.ToByte(subnets[1]);
            buf[27] = Convert.ToByte(subnets[2]);
            buf[28] = Convert.ToByte(subnets[3]);
            string[] serverips = msg.ServerIP.Split('.');
            buf[29] = Convert.ToByte(serverips[0]);
            buf[30] = Convert.ToByte(serverips[1]);
            buf[31] = Convert.ToByte(serverips[2]);
            buf[32] = Convert.ToByte(serverips[3]);
            buf[34] = (byte)((msg.ServerPort & 0xFF00) >> 8);
            buf[33] = (byte)(msg.ServerPort & 0x00FF);
            buf[35] = 0x02;
            buf[36] = 0x00;
            buf[37] = 0x00;
            int ncheck = 0;
            for (int i = 0; i < buf.Length - 1; i++)
            {
                ncheck += buf[i];
            }
            buf[38] = (byte)(ncheck);
            Write(nDeviceId, buf);
        }

        public bool GetWriteSetting(int nDeviceId)
        {
            foreach (var item in mSocketList)
            {
                if (item.Value.GetDeviceId() == nDeviceId)
                {
                    byte[] data = item.Value.GetReadReturnCommand();
                    if (data.Length == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void Write(int nDeviceId, byte[] buf)
        {
            foreach (var item in mSocketList)
            {
                if (item.Value.GetDeviceId() == nDeviceId)
                {
                    item.Key.Send(buf);
                    break;
                }
            }
        }
        public void OnNewConnection(INetworkSocket socket)
        {
            // 超出最大数就断开连接
            if (mSocketList.Count > mMaxConnectionCount)
            {
                socket.Disconnect();
            }
            mSocketList.Add(socket, new ClientMsg(socket));
        }
        public void OnReceived(INetworkSocket socket, byte[] buf)
        {
            ClientMsg client = mSocketList[socket];
            List<Byte> mRecv = client.GetBuf();
            mRecv.AddRange(buf);
            while (mRecv.Count > 9)
            {
                if (mRecv[0] == 0x55 && mRecv[1] == 0xAA)
                {
                    int dataLength = (mRecv[3] << 8 | mRecv[2]) + 2;
                    if (mRecv.Count >= dataLength)
                    {
                        byte[] data = new byte[dataLength];
                        mRecv.CopyTo(0, data, 0, dataLength);
                        int nDeviceId = data[5] << 8 | data[4];
                        client.SetDeviceId(nDeviceId);
                        //Console.WriteLine(nDeviceId.ToString());
                        switch (data[6])
                        {
                            case 0x01:
                                // 接收数据
                                #region " 0x01 "
                                DateTime now = DateTime.Now;
                                int nCount = dataLength / 9 - 1;
                                for (int i = 0; i < nCount; i++)
                                {
                                    int nTagId = data[13 + i*9]<<16 | data[12 + i*9]<<8 | data[11 + i*9];
                                    int nDeviceRssi = data[10 + i * 9] << 8 | data[9 + i * 9];
                                    int nActiveId = data[15 + i * 9] << 8 | data[16 + i * 9];
                                    int nTagRssi = data[17 + i * 9];
                                    // set value 
                                    TagMsg tagMsg = new TagMsg();
                                    tagMsg.ReciveDt = now;
                                    tagMsg.State = data[14 + i * 9];
                                    tagMsg.DeviceId = nDeviceId;
                                    tagMsg.DeviceRssi = nDeviceRssi;
                                    tagMsg.ActiveId = nActiveId;
                                    tagMsg.TagId = nTagId;
                                    tagMsg.TagRssi = nTagRssi;
                                    if (mMsg.Count >= mMaxTempMsgCount)
                                    {
                                        mMsg.RemoveAt(0);
                                    }
                                    // 过滤为0的数据
                                    if (tagMsg.ActiveId != 0)
                                    {
                                        mMsg.Add(tagMsg);
                                    }
                                }
                                #endregion
                                break;
                            case 0x02:
                                // 心跳
                                break;
                            case 0x03:
                                // 配置集中器参数指令
                                break;
                            case 0x04:
                                // 设置控制器参数指令
                                break;
                            case 0x05:
                                // 设置主机出厂指令
                                break;
                            case 0x0B:
                                // 配置基站网关参数指令
                                client.SetWriteReturnCommand(data);
                                break;
                            case 0x0C:
                                // 读取基站网关参数指令
                                client.SetReadReturnCommand(data);
                                break;
                            default:
                                break;
                        }
                        mRecv.RemoveRange(0, dataLength);
                        continue;
                    }
                    else 
                    {
                        // 数据长度不一样就跳出
                        break;
                    }
                }
                else
                {
                    mRecv.RemoveAt(0);
                }
            } // endwhile
        }

        #region  " override funcation"
        public void OnServerStarted(INetworkServer server, StartStatus status)
        {
            if (status == StartStatus.FAIL_ALREADY_STARTED || status == StartStatus.SUCCESS){}
            else{}
        }
        public bool OnAccept(INetworkServer server, IPInfo ipInfo)
        {
            return true;
        }
        public INetworkSocketCallback GetSocketCallback()
        {
            return this;
        }
        public void OnServerAccepted(INetworkServer server, INetworkSocket socket){ }
        public void OnServerStopped(INetworkServer server) {  }
        public void OnSent(INetworkSocket socket, SendStatus status, byte[] sentPacket)
        {
            switch (status)
            {
                case SendStatus.SUCCESS:
                    break;
                case SendStatus.FAIL_CONNECTION_CLOSING:
                    break;
                case SendStatus.FAIL_INVALID_PACKET:
                    break;
                case SendStatus.FAIL_NOT_CONNECTED:
                    break;
                case SendStatus.FAIL_SOCKET_ERROR:
                    break;
            }
        }
        public void OnDisconnect(INetworkSocket socket)
        {
            if (mSocketList.ContainsKey(socket))
            {
                mSocketList.Remove(socket);
            }
        }
        #endregion      
    }
}
