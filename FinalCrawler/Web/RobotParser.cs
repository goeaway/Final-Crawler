using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinalCrawler.Abstractions.Web;

namespace FinalCrawler.Web
{
    public class RobotParser : IRobotParser
    {
        public Task<bool> UriForbidden(Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}
