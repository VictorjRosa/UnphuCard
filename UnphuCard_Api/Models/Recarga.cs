using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Recarga
{
    public int RecId { get; set; }

    public decimal? RecMonto { get; set; }

    public DateTime? RecFecha { get; set; }

    public int? UsuId { get; set; }

    public int? MetPagId { get; set; }
}
