using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Pinterest
{
    public class Pin
    {
        public string created_at { get; set; }
        public object video_status_message { get; set; }
        public string title { get; set; }
        public int repin_count { get; set; }
        public Board board { get; set; }
        public string type { get; set; }
        public object attribution { get; set; }
        public object videos { get; set; }
        public Aggregated_Pin_Data aggregated_pin_data { get; set; }
        public string dominant_color { get; set; }
        public object story_pin_data_id { get; set; }
        public string description { get; set; }
        public object rich_metadata { get; set; }
        public object[] shopping_flags { get; set; }
        public object rich_summary { get; set; }
        public object product_pin_data { get; set; }
        public string link { get; set; }
        public string id { get; set; }
        public string domain { get; set; }
        public int comment_count { get; set; }
        public Images images { get; set; }
        public Pinner pinner { get; set; }
        public string image_signature { get; set; }
        public string tracking_params { get; set; }
        public string description_html { get; set; }
        public object video_status { get; set; }
        public Pin_Join pin_join { get; set; }
    }

    public class Board
    {
        public string type { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public bool is_collaborative { get; set; }
        public Owner owner { get; set; }
        public string id { get; set; }
    }

    public class Aggregated_Pin_Data
    {
        public Aggregated_Stats aggregated_stats { get; set; }
    }

    public class Aggregated_Stats
    {
        public int saves { get; set; }
        public int done { get; set; }
    }

    public class Images
    {
        public Orig orig { get; set; }
    }

    public class Orig
    {
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
    }

    public class Pinner
    {
        public string username { get; set; }
        public string type { get; set; }
        public string image_small_url { get; set; }
        public string image_large_url { get; set; }
        public string full_name { get; set; }
        public bool explicitly_followed_by_me { get; set; }
        public string first_name { get; set; }
        public string id { get; set; }
    }

    public class Pin_Join
    {
        public Annotations_With_Links annotations_with_links { get; set; }
        public string seo_description { get; set; }
        public object[] visual_descriptions { get; set; }
        public string[] visual_annotation { get; set; }
        public Canonical_Pin canonical_pin { get; set; }
        public object breadcrumbs { get; set; }
    }

    public class Annotations_With_Links
    {
        public YatchBoat YatchBoat { get; set; }
        public YachtDesign YachtDesign { get; set; }
        public BoatDesign BoatDesign { get; set; }
        public FastBoats FastBoats { get; set; }
        public CoolBoats CoolBoats { get; set; }
        public SpeedBoats SpeedBoats { get; set; }
        public PowerBoats PowerBoats { get; set; }
        public SmallBoats SmallBoats { get; set; }
    }

    public class YatchBoat
    {
        public string url { get; set; }
        public string name { get; set; }
    }

    public class YachtDesign
    {
        public string url { get; set; }
        public string name { get; set; }
    }

    public class BoatDesign
    {
        public string url { get; set; }
        public string name { get; set; }
    }

    public class FastBoats
    {
        public string url { get; set; }
        public string name { get; set; }
    }

    public class CoolBoats
    {
        public string url { get; set; }
        public string name { get; set; }
    }

    public class SpeedBoats
    {
        public string url { get; set; }
        public string name { get; set; }
    }

    public class PowerBoats
    {
        public string url { get; set; }
        public string name { get; set; }
    }

    public class SmallBoats
    {
        public string url { get; set; }
        public string name { get; set; }
    }

    public class Canonical_Pin
    {
        public string id { get; set; }
    }

}
