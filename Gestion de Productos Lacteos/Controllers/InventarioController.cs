using Microsoft.AspNetCore.Mvc;
using SistemaInventarioLacteos.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SistemaInventarioLacteos.Controllers
{
    public class InventarioController : Controller
    {
        // Datos simulados de inventario
        private static List<Inventario> _inventario = new List<Inventario>
        {
            new Inventario { IdInventario = 1, IdProducto = 1, StockActual = 25, StockMinimo = 10,
                Producto = new Producto { IdProducto = 1, NombreProducto = "Queso Fresco", Categoria = "Lácteos" } },
            new Inventario { IdInventario = 2, IdProducto = 2, StockActual = 50, StockMinimo = 20,
                Producto = new Producto { IdProducto = 2, NombreProducto = "Leche Entera", Categoria = "Lácteos" } },
            new Inventario { IdInventario = 3, IdProducto = 3, StockActual = 8, StockMinimo = 15,
                Producto = new Producto { IdProducto = 3, NombreProducto = "Yogur Natural", Categoria = "Lácteos" } },
            new Inventario { IdInventario = 4, IdProducto = 4, StockActual = 0, StockMinimo = 5,
                Producto = new Producto { IdProducto = 4, NombreProducto = "Queso Duro", Categoria = "Lácteos" } }
        };

        // GET: Inventario
        public IActionResult Index()
        {
            // Calcular stock bajo para el badge
            var stockBajo = _inventario.Count(i => i.StockActual <= i.StockMinimo);
            ViewBag.StockBajoCount = stockBajo;

            return View(_inventario);
        }

        // POST: Inventario/RegistrarMovimiento
        [HttpPost]
        public IActionResult RegistrarMovimiento([FromBody] MovimientoInventario movimiento)
        {
            if (movimiento == null || movimiento.Cantidad <= 0)
            {
                return BadRequest(new { success = false, message = "Datos inválidos" });
            }

            var inventario = _inventario.FirstOrDefault(i => i.IdProducto == movimiento.IdProducto);
            if (inventario != null)
            {
                if (movimiento.TipoMovimiento == "Entrada")
                {
                    inventario.StockActual += movimiento.Cantidad;
                }
                else if (movimiento.TipoMovimiento == "Salida")
                {
                    if (inventario.StockActual >= movimiento.Cantidad)
                    {
                        inventario.StockActual -= movimiento.Cantidad;
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Stock insuficiente" });
                    }
                }
                TempData["Success"] = $"Movimiento registrado: {movimiento.TipoMovimiento} de {movimiento.Cantidad} unidades";
                return Ok(new { success = true });
            }

            return BadRequest(new { success = false, message = "Producto no encontrado" });
        }
    }
}