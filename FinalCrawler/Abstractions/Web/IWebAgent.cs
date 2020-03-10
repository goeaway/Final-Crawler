using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FinalCrawler.Abstractions.Web
{
    public interface IWebAgent
    {
        Task<string> MakeRequest(Uri uri);
    }
}
