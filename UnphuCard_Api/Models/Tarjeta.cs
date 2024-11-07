using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Tarjeta
{
    public int TarjId { get; set; }

    public int TarjCodigo { get; set; }

    public DateTime? TarjFecha { get; set; }

    public int? StatusId { get; set; }

    public int? UsuId { get; set; }
}
