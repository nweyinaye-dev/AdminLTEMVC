using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCAdminLTE.ApiServices;
using MVCAdminLTE.Models;
using System.Diagnostics;

namespace MVCAdminLTE.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly WeatherApiService _wfApiService;

        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, WeatherApiService weatherApiService)
        {
            _logger = logger;
            _wfApiService = weatherApiService;
        }

        public IActionResult Index()
        {
            var weatherForecast = _wfApiService.GetWeatherAsync().Result;
            return View();
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
