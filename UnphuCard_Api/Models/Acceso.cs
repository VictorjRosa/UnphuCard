using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Acceso
{
    public int AccesId { get; set; }

    public DateTime? AccesFecha { get; set; }

    public int? UsuId { get; set; }

    public int? AulaId { get; set; }

    public int? StatusId { get; set; }
}
