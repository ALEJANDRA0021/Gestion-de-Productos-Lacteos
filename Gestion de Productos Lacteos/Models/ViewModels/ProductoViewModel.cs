using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioLacteos.Models.ViewModels
{
    public class ProductoViewModel
    {
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres")]
        [Display(Name = "Nombre del Producto")]
        public string NombreProducto { get; set; } = null!;

        [Display(Name = "Categoría")]
        public string? Categoria { get; set; }

        [StringLength(200, ErrorMessage = "La descripción no puede exceder los 200 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio de compra es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio de Compra")]
        public decimal? PrecioCompra { get; set; }

        [Required(ErrorMessage = "El precio de venta es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio de Venta")]
        public decimal? PrecioVenta { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Datos de inventario
        [Display(Name = "Stock Actual")]
        public int? StockActual { get; set; } = 0;

        [Range(0, 999999, ErrorMessage = "El stock mínimo no puede ser negativo")]
        [Display(Name = "Stock Mínimo")]
        public int? StockMinimo { get; set; } = 10;
    }
}