using System;

namespace Gestion_de_Productos_Lacteos.Models
{
    public class LoteProximoAVencer
    {
        public Lote Lote { get; set; }
        public Producto Producto { get; set; }

        public int DiasRestantes => Lote.FechaVencimiento.HasValue
            ? (Lote.FechaVencimiento.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days
            : 0;
    }
}