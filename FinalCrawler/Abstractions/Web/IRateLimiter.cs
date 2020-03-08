using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FinalCrawler.Abstractions.Web
{
    public interface IRateLimiter
    {
        Task HoldIfRequired(Uri uri);
    }
}
