using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RfidNet;

namespace carMonitor
{
    public class Temp
    {
        public Temp(TagMsg msg)
        {
            this.Tagmsg = msg;
            Count = 1;
        }
        public TagMsg Tagmsg { get; set; }
        public long Count { get; set; }
    }
}
