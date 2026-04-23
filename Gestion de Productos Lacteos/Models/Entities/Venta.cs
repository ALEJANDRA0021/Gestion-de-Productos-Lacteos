using Gestion_de_Productos_Lacteos.Models;
using System;
using System.Collections.Generic;

namespace SistemaInventarioLacteos.Models.Entities
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public DateTime? FechaVenta { get; set; }
        public int? IdCliente { get; set; }
        public int? IdUsuario { get; set; }
        public string? TipoComprobante { get; set; }
        public decimal? Total { get; set; }

        // Relaciones
        public virtual Cliente? Cliente { get; set; }
        public virtual Usuario? Usuario { get; set; }
        public virtual ICollection<DetalleVentum>? DetalleVenta { get; set; }
    }
}