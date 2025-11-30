using System.Diagnostics;
using Bookify.wep.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.wep.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        //عشان عرض الغرف بتاعت صفحة الريسية تبقي حقيقة الداتا 




        public IActionResult Index()
        {
            // ???? ?? Views/Home/Index.cshtml
            return View();
        }

        // Action Method ??????? ???? ???? "About"
        public IActionResult About()
        {
            // ????? ???????? ?? ??? Views/Home/About.cshtml
            return View();
        }

        // Action Method ??????? ???? ???? "Contact"
        public IActionResult Contact()
        {
            // ????? ???????? ?? ??? Views/Home/Contact.cshtml
            return View();
        }

        public IActionResult Privacy()
        {
            // ???? ?? Views/Home/Privacy.cshtml
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}