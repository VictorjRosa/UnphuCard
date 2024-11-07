using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Aula
{
    public int AulaId { get; set; }

    public string? AulaDescripcion { get; set; }

    public string? AulaUbicacion { get; set; }

    public string? AulaSensor { get; set; }
}
