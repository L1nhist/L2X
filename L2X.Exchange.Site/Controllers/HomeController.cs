using L2X.Exchange.Admin.Models;
using System.Diagnostics;

namespace L2X.Exchange.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index(string type)
        {
            return View(model: type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
