using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Dispositivo
{
    public int DispId { get; set; }

    public string? DispAndroidId { get; set; }

    public DateTime? DispFecha { get; set; }

    public int? EstId { get; set; }
}
