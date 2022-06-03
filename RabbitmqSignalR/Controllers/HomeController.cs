using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitmqSignalR.Models;
using RabbitmqSignalR.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitmqSignalR.Controllers
{
    public class HomeController : Controller
    {

        public static List<Stock> stocList = new List<Stock>(){
                new Stock(){ID=1,Name="NetasTelekom",Value=Decimal.Parse("14.56")},
                new Stock(){ID=2,Name="Bitcoin",Value=Decimal.Parse("41,583.99")},
                new Stock(){ID=3,Name="Ethereum",Value=Decimal.Parse("1863.92")}
            };

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var data = GetDummyData();
            return View(data);
        }

        [HttpPost]
        public IActionResult Push(Stock stock)
        {
            UpdateDummyList(stock);
            RabbitMQPost rabbitMq = new RabbitMQPost(stock);
            Console.WriteLine(rabbitMq.Post());
            return RedirectToAction("Index");
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

        public List<Stock> GetDummyData()
        {
            return stocList;
        }

        public void UpdateDummyList(Stock stock)
        {
            int index = stocList.FindIndex(st => st.ID == stock.ID);
            stocList[index] = stock;
        }

    }
}
