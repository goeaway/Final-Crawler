using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinalCrawler.ProceduralSite.Models;
using FinalCrawler.ProceduralSite.Services;

namespace FinalCrawler.ProceduralSite.Controllers
{
    public class HomeController : Controller
    {
        public readonly IRandomiserService _randomiser;

        public HomeController(IRandomiserService randomiserService)
        {
            _randomiser = randomiserService;
        }

        [HttpGet("{*url}")]
        public async Task<IActionResult> Index(int wait)
        {
            await Task.Delay(wait > -1 ? wait : _randomiser.GetRandomWait());

            return View(new DataModel
            {
                URLs = _randomiser.GetRandomUrls("").Select(u => u + "&wait=" + wait),
                Emails = _randomiser.GetRandomEmails(),
                Images = _randomiser.GetRandomImages()
            });
        }
    }
}
