using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.ShopAdmin.Controllers
{
    public class DashboardController : Controller
    {
        [Area(nameof(ShopAdmin))]
        public IActionResult Index()
        {
            return View();
        }
    }
}
