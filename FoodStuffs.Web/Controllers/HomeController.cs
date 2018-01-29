using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FoodStuffs.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IHostingEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        [Route("/Error")]
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Index()
        {
            ViewBag.UseWebpackDevServer = _environment.IsEnvironment("Development");
            ViewBag.ApplicationName = _configuration["ApplicationName"];

            return View();
        }

        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;
    }
}