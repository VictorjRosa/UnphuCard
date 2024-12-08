using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class VwRecarga
{
    public int? IdDelUsuario { get; set; }

    public DateTime? Fecha { get; set; }

    public string Descripción { get; set; } = null!;

    public string? MétodoDePago { get; set; }

    public decimal? Monto { get; set; }
}
