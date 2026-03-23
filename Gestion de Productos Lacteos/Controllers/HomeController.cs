using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SistemaInventarioLacteos.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.TotalProductos = 8;
            ViewBag.StockBajo = 2;
            ViewBag.VentasHoy = 125.50m;
            ViewBag.ProductosPorVencer = 3;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }
    }
}