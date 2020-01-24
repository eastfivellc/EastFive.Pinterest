using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Pinterest
{
    public class ResourceResponse
    {
        public string name { get; set; }
        public Options options { get; set; }
        public Response response { get; set; }
        public string nextBookmark { get; set; }
    }

}
