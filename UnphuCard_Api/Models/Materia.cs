using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Materia
{
    public int MatId { get; set; }

    public string? MatCodigo { get; set; }

    public string? MatDescripcion { get; set; }

    public int? UsuId { get; set; }
}
