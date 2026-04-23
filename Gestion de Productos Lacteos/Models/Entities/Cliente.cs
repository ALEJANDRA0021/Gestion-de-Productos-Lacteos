using Gestion_de_Productos_Lacteos.Models;
using System.Collections.Generic;

namespace SistemaInventarioLacteos.Models.Entities
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Correo { get; set; }
        public string? TipoCliente { get; set; }
        public string? Dui { get; set; }
        public string? Nit { get; set; }
        public string? Ncr { get; set; }

        // Relaciones
        public virtual ICollection<Venta>? Venta { get; set; }
    }
}