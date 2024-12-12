using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Establecimiento
{
    public int EstId { get; set; }

    public string? EstDescripcion { get; set; }

    public string? EstUbicacion { get; set; }
}
