using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Compra
{
    public int CompId { get; set; }

    public decimal? CompMonto { get; set; }

    public DateTime? CompFecha { get; set; }

    public int? UsuId { get; set; }

    public int? EstId { get; set; }

    public int? MetPagId { get; set; }

    public int? SesionId { get; set; }
}
