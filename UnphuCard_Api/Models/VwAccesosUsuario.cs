using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class VwAccesosUsuario
{
    public int Id { get; set; }

    public string? NombreCompleto { get; set; }

    public string? Matrícula { get; set; }

    public DateTime? FechaDeIntento { get; set; }

    public string? Aula { get; set; }

    public string? Estado { get; set; }
}
