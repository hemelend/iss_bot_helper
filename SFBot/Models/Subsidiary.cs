using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFBot.Models
{
    [Serializable]
    public class Subsidiary
    {
        public string Region { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }
    }
}