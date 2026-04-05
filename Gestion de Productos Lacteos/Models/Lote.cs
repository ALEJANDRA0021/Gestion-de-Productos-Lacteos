using System;
using System.Collections.Generic;

namespace Gestion_de_Productos_Lacteos.Models;

public partial class Lote
{
    public int IdLote { get; set; }

    public int? IdProducto { get; set; }

    public string? NumeroLote { get; set; }

    public DateOnly? FechaProduccion { get; set; }

    public DateOnly? FechaVencimiento { get; set; }

    public int? Cantidad { get; set; }

    public virtual ICollection<AlertasVencimiento> AlertasVencimientos { get; set; } = new List<AlertasVencimiento>();

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual Producto? IdProductoNavigation { get; set; }
}
