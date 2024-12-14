using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class VwProducto
{
    public int IdDelProducto { get; set; }

    public string? NombreDelProducto { get; set; }

    public decimal? PrecioDelProducto { get; set; }

    public string? ImagenDelProducto { get; set; }

    public int IdDelEstadoDelProducto { get; set; }

    public string? DescripciónDelEstadoDelProducto { get; set; }

    public int IdDeLaCategoríaDelProducto { get; set; }

    public string? DescripciónDeLaCategoríaDelProducto { get; set; }
}
