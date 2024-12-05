using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class VwCarritoCompra
{
    public int IdDeCompra { get; set; }

    public DateTime? FechaDeCompra { get; set; }

    public int? CantidadDeCompra { get; set; }

    public int IdDelProducto { get; set; }

    public string? NombreDelProducto { get; set; }

    public decimal? PrecioDelProducto { get; set; }

    public int SesiónId { get; set; }

    public string? SesiónToken { get; set; }
}
