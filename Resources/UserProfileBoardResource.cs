using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EastFive.Pinterest
{
    public class UserProfileBoardResource
    {
        public bool collaborated_by_me { get; set; }
        public bool followed_by_me { get; set; }
        public object[] access { get; set; }
        public int pin_count { get; set; }
        public Owner owner { get; set; }
        public string image_thumbnail_url { get; set; }
        public string[] pin_thumbnail_urls { get; set; }
        public Cover_Images cover_images { get; set; }
        public bool is_collaborative { get; set; }
        public string privacy { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public string layout { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Cover_Images
    {
        public _216X146 _216x146 { get; set; }
        public _400X300 _400x300 { get; set; }
    }

    public class _216X146
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class _400X300
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

}
