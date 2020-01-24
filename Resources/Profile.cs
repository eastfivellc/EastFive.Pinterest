using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Pinterest
{
    public class Profile
    {
        public bool domain_verified { get; set; }
        public string country { get; set; }
        public string locale { get; set; }
        public object domain_url { get; set; }
        public string image_xlarge_url { get; set; }
        public string location { get; set; }
        public string full_name { get; set; }
        public int pin_count { get; set; }
        public object impressum_url { get; set; }
        public string about { get; set; }
        public int follower_count { get; set; }
        public string username { get; set; }
        public int following_count { get; set; }
        public int board_count { get; set; }
        public string noindex_reason { get; set; }
        public bool indexed { get; set; }
    }
}
