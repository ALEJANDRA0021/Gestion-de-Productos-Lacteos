using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gestion_de_Productos_Lacteos.Models.ViewModels
{
    public class VentaRegistroViewModel
    {
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public string? NombreCliente { get; set; }
        public decimal? Total { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        [Display(Name = "Cliente")]
        public int? IdCliente { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de comprobante")]
        [Display(Name = "Tipo de Comprobante")]
        public string? TipoComprobante { get; set; }

        public List<DetalleVentaViewModel> Detalles { get; set; } = new List<DetalleVentaViewModel>();

        public IEnumerable<SelectListItem>? Clientes { get; set; }
        public IEnumerable<SelectListItem>? TiposComprobante { get; set; }
        public IEnumerable<SelectListItem>? Productos { get; set; }
    }

    public class DetalleVentaViewModel
    {
        public int IdProducto { get; set; }
        public string? NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal => Cantidad * Precio;
    }
}