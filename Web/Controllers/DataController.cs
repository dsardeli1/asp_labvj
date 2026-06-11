using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManageApp.Controllers
{
    [Authorize]
    public class DataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
