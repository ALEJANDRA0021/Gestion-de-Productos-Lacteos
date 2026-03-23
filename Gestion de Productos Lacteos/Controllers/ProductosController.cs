using Microsoft.AspNetCore.Mvc;
using SistemaInventarioLacteos.Models.Entities;
using SistemaInventarioLacteos.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace SistemaInventarioLacteos.Controllers
{
    public class ProductosController : Controller
    {
        // Datos temporales en memoria (simulados)
        private static List<Producto> _productos = new List<Producto>
        {
            new Producto
            {
                IdProducto = 1,
                NombreProducto = "Queso Fresco",
                Categoria = "Lácteos",
                Descripcion = "Queso fresco pasteurizado",
                PrecioVenta = 3.50m,
                PrecioCompra = 2.50m,
                Activo = true,
                Inventario = new Inventario { StockActual = 25, StockMinimo = 10 }
            },
            new Producto
            {
                IdProducto = 2,
                NombreProducto = "Leche Entera",
                Categoria = "Lácteos",
                Descripcion = "Leche entera 1 litro",
                PrecioVenta = 1.20m,
                PrecioCompra = 0.90m,
                Activo = true,
                Inventario = new Inventario { StockActual = 50, StockMinimo = 20 }
            },
            new Producto
            {
                IdProducto = 3,
                NombreProducto = "Yogur Natural",
                Categoria = "Lácteos",
                Descripcion = "Yogur natural 1kg",
                PrecioVenta = 2.80m,
                PrecioCompra = 2.00m,
                Activo = true,
                Inventario = new Inventario { StockActual = 8, StockMinimo = 15 }
            }
        };

        // GET: Productos
        public IActionResult Index()
        {
            return View(_productos);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                producto.IdProducto = _productos.Max(p => p.IdProducto) + 1;
                producto.Activo = true;
                producto.Inventario = new Inventario
                {
                    IdInventario = producto.IdProducto,
                    IdProducto = producto.IdProducto,
                    StockActual = 0,
                    StockMinimo = 10
                };
                _productos.Add(producto);
                TempData["Success"] = "Producto creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Productos/Edit/5
        public IActionResult Edit(int id)
        {
            var producto = _productos.FirstOrDefault(p => p.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = _productos.FirstOrDefault(p => p.IdProducto == id);
                if (existing != null)
                {
                    existing.NombreProducto = producto.NombreProducto;
                    existing.Categoria = producto.Categoria;
                    existing.Descripcion = producto.Descripcion;
                    existing.PrecioVenta = producto.PrecioVenta;
                    existing.PrecioCompra = producto.PrecioCompra;
                    TempData["Success"] = "Producto actualizado exitosamente";
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var producto = _productos.FirstOrDefault(p => p.IdProducto == id);
            if (producto != null)
            {
                producto.Activo = false;
                TempData["Success"] = "Producto eliminado exitosamente";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}