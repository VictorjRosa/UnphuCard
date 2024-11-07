using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Producto
{
    public int ProdId { get; set; }

    public string? ProdDescripcion { get; set; }

    public decimal? ProdPrecio { get; set; }

    public string? ProdImagenes { get; set; }

    public int? StatusId { get; set; }

    public int? CatProdId { get; set; }
}
