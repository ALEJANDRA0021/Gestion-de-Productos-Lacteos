using Microsoft.AspNetCore.Mvc;

namespace Gestion_de_Productos_Lacteos.Controllers
{
    public class VentasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}