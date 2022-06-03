using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitSignalRClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitSignalRClient.Controllers
{
    public class HomeController : Controller
    {

        public static List<Stock> stocList = new List<Stock>()
        {
            new Stock(){ID=1,Name="NetasTelekom",Value=Decimal.Parse("14.56")},
            new Stock(){ID=2,Name="Bitcoin",Value=Decimal.Parse("41,583.99")},
            new Stock(){ID=3,Name="Ethereum",Value=Decimal.Parse("1863.92")}
        };
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Detail(int ID)
        {
            Stock stock = stocList.FirstOrDefault(s => s.ID == ID);
            return View(stock);
        }

        public IActionResult Index()
        {
            return View(stocList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
