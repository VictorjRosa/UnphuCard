using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class TarjetasProvisionale
{
    public int TarjProvId { get; set; }

    public string? TarjProvCodigo { get; set; }

    public DateTime? TarjProvFecha { get; set; }

    public DateTime? TarjProvFechaExpiracion { get; set; }

    public int? StatusId { get; set; }

    public int? UsuId { get; set; }
}
