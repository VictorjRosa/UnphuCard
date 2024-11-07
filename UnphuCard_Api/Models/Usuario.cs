using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Usuario
{
    public int UsuId { get; set; }

    public int UsuCodigo { get; set; }

    public string? UsuNombre { get; set; }

    public string? UsuApellido { get; set; }

    public string? UsuMatricula { get; set; }

    public string? UsuUsuario { get; set; }

    public string? UsuCorreo { get; set; }

    public string? UsuContraseña { get; set; }

    public decimal? UsuSaldo { get; set; }

    public int? StatusId { get; set; }

    public int? RolId { get; set; }
}
