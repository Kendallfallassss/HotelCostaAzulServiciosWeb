using Microsoft.AspNetCore.Mvc;

namespace HotelCostaAzul.Web.Controllers
{
    public class ClienteController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
