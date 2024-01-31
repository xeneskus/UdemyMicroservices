using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers
{
    public class BasketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
