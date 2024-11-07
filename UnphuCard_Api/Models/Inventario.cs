using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Inventario
{
    public int InvId { get; set; }

    public int? InvCantidad { get; set; }

    public DateTime? InvFecha { get; set; }

    public int? EstId { get; set; }

    public int? ProdId { get; set; }
}
