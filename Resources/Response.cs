using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Pinterest
{
    public class Response
    {
        public string status { get; set; }
        public int code { get; set; }
        public dynamic data { get; set; }
        public string message { get; set; }
        public int http_status { get; set; }
    }
}
