using Microsoft.AspNetCore.Mvc;
using SistemaInventarioLacteos.Models.Entities;
using SistemaInventarioLacteos.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace SistemaInventarioLacteos.Controllers
{
    public class VentasController : Controller
    {
        // Datos temporales de productos (simulados)
        private static List<Producto> _productos = new List<Producto>
        {
            new Producto { IdProducto = 1, NombreProducto = "Queso Fresco", PrecioVenta = 3.50m, Inventario = new Inventario { StockActual = 25 } },
            new Producto { IdProducto = 2, NombreProducto = "Leche Entera", PrecioVenta = 1.20m, Inventario = new Inventario { StockActual = 50 } },
            new Producto { IdProducto = 3, NombreProducto = "Yogur Natural", PrecioVenta = 2.80m, Inventario = new Inventario { StockActual = 8 } },
            new Producto { IdProducto = 4, NombreProducto = "Mantequilla", PrecioVenta = 4.50m, Inventario = new Inventario { StockActual = 15 } },
            new Producto { IdProducto = 5, NombreProducto = "Crema", PrecioVenta = 2.30m, Inventario = new Inventario { StockActual = 12 } }
        };

        private static List<Cliente> _clientes = new List<Cliente>
        {
            new Cliente { IdCliente = 1, Nombre = "Consumidor Final", TipoCliente = "Consumidor Final" },
            new Cliente { IdCliente = 2, Nombre = "Juan Pérez", TipoCliente = "Contribuyente", NIT = "1234-567890-123-4" },
            new Cliente { IdCliente = 3, Nombre = "María López", TipoCliente = "Contribuyente", NIT = "5678-123456-789-0" }
        };

        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewBag.Clientes = _clientes;
            return View();
        }

        // GET: Ventas/BuscarProductos
        [HttpGet]
        public IActionResult BuscarProductos(string term)
        {
            if (string.IsNullOrEmpty(term))
                return Json(new List<object>());

            var resultados = _productos
                .Where(p => p.NombreProducto.ToLower().Contains(term.ToLower()) && p.Activo)
                .Select(p => new
                {
                    idProducto = p.IdProducto,
                    nombreProducto = p.NombreProducto,
                    precioVenta = p.PrecioVenta,
                    stockActual = p.Inventario?.StockActual ?? 0
                })
                .Take(10)
                .ToList();

            return Json(resultados);
        }

        // POST: Ventas/RegistrarVenta
        [HttpPost]
        public IActionResult RegistrarVenta([FromBody] VentaViewModel venta)
        {
            if (venta == null || venta.Detalles == null || !venta.Detalles.Any())
            {
                return BadRequest(new { success = false, message = "No hay productos en la venta" });
            }

            // Aquí iría la lógica para guardar en base de datos
            // Por ahora simulamos éxito
            TempData["Success"] = $"Venta registrada exitosamente. Total: ${venta.Total}";
            return Ok(new { success = true });
        }

        // GET: Ventas/Index (Historial)
        public IActionResult Index()
        {
            // Datos simulados de ventas anteriores
            var ventas = new List<Venta>
            {
                new Venta { IdVenta = 1, FechaVenta = System.DateTime.Now.AddDays(-1), Total = 15.50m, Cliente = _clientes[0] },
                new Venta { IdVenta = 2, FechaVenta = System.DateTime.Now.AddDays(-2), Total = 23.80m, Cliente = _clientes[1] },
                new Venta { IdVenta = 3, FechaVenta = System.DateTime.Now.AddDays(-3), Total = 8.40m, Cliente = _clientes[2] }
            };
            return View(ventas);
        }
    }
}