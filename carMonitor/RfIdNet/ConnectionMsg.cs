using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RfidNet
{
    public class ConnectionMsg
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public int DeviceId { get; set; }
    }
}
