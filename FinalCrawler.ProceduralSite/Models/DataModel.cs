using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalCrawler.ProceduralSite.Models
{
    public class DataModel
    {
        public IEnumerable<string> URLs { get; set; }
        public IEnumerable<string> Emails { get; set; }
        public IEnumerable<string> Images { get; set; }
    }
}
