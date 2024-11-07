using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class VwComprasUsuario
{
    public int IdCompra { get; set; }

    public decimal? MontoDeCompra { get; set; }

    public DateTime? FechaDeLaCompra { get; set; }

    public string? NombreCompleto { get; set; }

    public string? Matrícula { get; set; }

    public string? EstadoDelUsuario { get; set; }

    public string? NombreDelEstablecimiento { get; set; }

    public string? MétodoDePago { get; set; }
}
