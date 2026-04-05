using System;
using System.Collections.Generic;

namespace Gestion_de_Productos_Lacteos.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public string? Correo { get; set; }

    public string? TipoCliente { get; set; }

    public string? Dui { get; set; }

    public string? Nit { get; set; }

    public string? Nrc { get; set; }

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
