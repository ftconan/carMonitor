using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RfidNet
{
    public class TagMsg
    {
        public int DeviceId { get; set; }
        public int ActiveId { get; set; }
        public int TagId { get; set; }
        public int DeviceRssi { get; set; }
        public int TagRssi { get; set; }
        public byte State { get; set; }
        public DateTime ReciveDt { get; set; }
       
    }
}
