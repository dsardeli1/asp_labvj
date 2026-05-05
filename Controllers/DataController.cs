using Microsoft.AspNetCore.Mvc;

namespace TaskManageApp.Controllers
{
    public class DataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
