using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioLacteos.Models.ViewModels
{
    public class VentaViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        public int IdCliente { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;

        public string TipoComprobante { get; set; } = "Factura";

        public decimal Total { get; set; }

        // Lista de productos en la venta
        public List<DetalleVentaViewModel> Detalles { get; set; } = new List<DetalleVentaViewModel>();
    }

    public class DetalleVentaViewModel
    {
        [Required]
        public int IdProducto { get; set; }

        public string NombreProducto { get; set; }

        [Required]
        [Range(1, 999999, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Precio { get; set; }

        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }
    }

    // ViewModel para búsqueda de productos
    public class ProductoBusquedaViewModel
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioVenta { get; set; }
        public int StockActual { get; set; }
        public string Categoria { get; set; }
    }
}