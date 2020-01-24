using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Pinterest
{
    public class BoardInitialData
    {
        public bool rebuildStoreOnClient { get; set; }
        public ResourceResponse[] resourceResponses { get; set; }
        public Routedata routeData { get; set; }
    }

    

    

}
