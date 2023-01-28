using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iKuaiManage
{
    class ResultData
    {
        public Data Data { get; set; }
        public string ErrMsg { get; set; }
        public int Result { get; set; }
    }
    class Data
    {
        public int total { get; set; }
        public L2TP[] data { get; set; }
    }
}
