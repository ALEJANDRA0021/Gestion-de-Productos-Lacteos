using System;
using System.Collections.Generic;

namespace Gestion_de_Productos_Lacteos.Models;

public partial class DetalleCompra
{
    public int IdDetalleCompra { get; set; }

    public int? IdCompra { get; set; }

    public int? IdLote { get; set; }

    public int? IdProducto { get; set; }

    public int? Cantidad { get; set; }

    public decimal? Precio { get; set; }

    public decimal? Subtotal { get; set; }

    public virtual Compra? IdCompraNavigation { get; set; }

    public virtual Lote? IdLoteNavigation { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
