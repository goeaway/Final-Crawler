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

        public async Task<IActionResult> Index(int seed, int wait)
        {
            await Task.Delay(wait);

            return View(new DataModel
            {
                URLs = _randomiser.GetRandomUrls(seed).Select(u => u + "&wait=" + wait),
                Emails = _randomiser.GetRandomEmails(seed),
                Images = _randomiser.GetRandomImages(seed)
            });
        }

        [HttpGet("area")]
        public Task<IActionResult> Area(int seed, int wait)
        {
            return Index(seed, wait);
        }
    }
}
