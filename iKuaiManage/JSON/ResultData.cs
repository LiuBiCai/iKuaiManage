using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iKuaiManage
{
    class ResultDataL2tp
    {
        public DataL2tp Data { get; set; }
        public string ErrMsg { get; set; }
        public int Result { get; set; }
    }
    class DataL2tp
    {
        public int total { get; set; }
        public L2TP[] data { get; set; }
    }
    class ResultDataStreamIpport
    {
        public DataStreamIpport Data { get; set; }
        public string ErrMsg { get; set; }
        public int Result { get; set; }
    }
    class DataStreamIpport
    {
        public int total { get; set; }
        public StreamIpport[] data { get; set; }
    }

}
