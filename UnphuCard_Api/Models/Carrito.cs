using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Carrito
{
    public int CarId { get; set; }

    public DateTime? CarFecha { get; set; }

    public int? CarCantidad { get; set; }

    public int? ProdId { get; set; }

    public int? SesionId { get; set; }
}
