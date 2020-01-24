using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Pinterest
{
    public class UserProfileBaseResource
    {
        public User user { get; set; }
        public Page_Metadata page_metadata { get; set; }
        public string full_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public Profile profile { get; set; }
    }

    
    public class Page_Metadata
    {
        public object[][] links { get; set; }
        public string locale { get; set; }
        public Metatags metatags { get; set; }
        public string canonical_domain { get; set; }
    }
}
