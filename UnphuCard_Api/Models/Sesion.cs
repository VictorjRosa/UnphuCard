using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Sesion
{
    public int SesionId { get; set; }

    public string? SesionToken { get; set; }

    public DateTime? SesionFecha { get; set; }

    public int? UsuId { get; set; }
}
