using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinalCrawler.ProceduralSite.Models;

namespace FinalCrawler.ProceduralSite.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("{*url}")]
        public IActionResult Index()
        {
            // get info about what to do from url

            // wait for some amount of milliseconds here to simulate a real server

            // return a length of new links, emails, images, etc

            // the view will display them for the crawler to find

            return View();
        }
    }
}
