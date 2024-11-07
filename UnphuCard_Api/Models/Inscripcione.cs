using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Inscripcione
{
    public int InsId { get; set; }

    public string? InsCuatrimestre { get; set; }

    public int? StatusId { get; set; }

    public int? UsuId { get; set; }

    public int? MatId { get; set; }
}
