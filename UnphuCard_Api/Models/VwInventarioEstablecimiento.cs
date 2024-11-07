using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class VwInventarioEstablecimiento
{
    public int Id { get; set; }

    public int? CantidadEnElInventario { get; set; }

    public DateTime? FechaDeEntrada { get; set; }

    public int IdDelEstablecimiento { get; set; }

    public string? NombreDelEstablecimiento { get; set; }

    public int IdDelProducto { get; set; }

    public string? NombreDelProducto { get; set; }

    public decimal? PrecioDelProducto { get; set; }

    public string? ImagenDelProducto { get; set; }

    public string? EstadoDelProducto { get; set; }
}
