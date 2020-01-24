using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Pinterest
{
    public class Owner
    {
        public string id { get; set; }
        public string gender { get; set; }
        public string username { get; set; }
        public string image_medium_url { get; set; }
        public bool is_partner { get; set; }
        public string country { get; set; }
        public bool indexed { get; set; }
        public string full_name { get; set; }
        public string first_name { get; set; }
        public bool blocked_by_me { get; set; }
        public object domain_url { get; set; }
        public string locale { get; set; }
        public string image_small_url { get; set; }
        public string image_large_url { get; set; }
        public string image_xlarge_url { get; set; }
        public bool is_tastemaker { get; set; }
        public string type { get; set; }
        public string last_name { get; set; }
    }
}
