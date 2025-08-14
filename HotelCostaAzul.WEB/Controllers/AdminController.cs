using Microsoft.AspNetCore.Mvc;

namespace HotelCostaAzul.Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
