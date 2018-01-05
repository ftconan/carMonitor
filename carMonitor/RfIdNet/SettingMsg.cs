using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RfidNet
{
    public class SettingMsg
    {
        public string LocalIP { get; set; }
        public short LocalPort { get; set; }
        public string GateWay { get; set; }
        public string MacAddress { get; set; }
        public string ServerIP { get; set; }
        public short ServerPort { get; set; }
        public int DeviceId { get; set; }
        public string SubNet { get; set; }
        public byte FValue { get; set; }
    }
}
