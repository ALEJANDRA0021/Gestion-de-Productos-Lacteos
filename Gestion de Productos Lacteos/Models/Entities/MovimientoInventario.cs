namespace SistemaInventarioLacteos.Models.Entities
{
    public class MovimientoInventario
    {
        public int IdProducto { get; set; }
        public string TipoMovimiento { get; set; } = null!;
        public int Cantidad { get; set; }
        public string? Descripcion { get; set; }
    }
}