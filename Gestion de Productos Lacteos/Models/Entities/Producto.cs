using Gestion_de_Productos_Lacteos.Models;
using System;
using System.Collections.Generic;

namespace SistemaInventarioLacteos.Models.Entities
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = null!;
        public string? Categoria { get; set; }
        public string? Descripcion { get; set; }
        public decimal? PrecioCompra { get; set; }
        public decimal? PrecioVenta { get; set; }
        public bool Activo { get; set; } = true;

        // Relaciones
        public virtual Inventario? Inventario { get; set; }
        public virtual ICollection<DetalleVentum>? DetalleVenta { get; set; }
        public virtual ICollection<DetalleCompra>? DetalleCompras { get; set; }
        public virtual ICollection<Lote>? Lotes { get; set; }
        public virtual ICollection<MovimientosInventario>? MovimientosInventarios { get; set; }
    }
}