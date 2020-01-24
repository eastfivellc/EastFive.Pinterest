using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Pinterest
{
    public class Options
    {
        public bool? isPrefetch;
        public string[] bookmarks { get; set; }
        public string field_set_key { get; set; }
        public bool force_node { get; set; }
        public string username { get; set; }
        public string slug { get; set; }
        public string get_page_metadata { get; set; }
        public string board_id { get; set; }
        public string board_title { get; set; }
        public object rank_with_query { get; set; }
        public bool prepend { get; set; }
        public Owner owner { get; set; }
        public int pin_count { get; set; }
        public bool add_vase { get; set; }
        public bool filter_section_pins { get; set; }
        public int page_size { get; set; }
    }
}
