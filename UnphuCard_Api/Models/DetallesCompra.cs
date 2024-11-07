using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class DetallesCompra
{
    public int DetCompId { get; set; }

    public int? DetCompCantidad { get; set; }

    public decimal? DetCompPrecio { get; set; }

    public int? CompId { get; set; }

    public int? ProdId { get; set; }

    public int? SesionId { get; set; }
}
