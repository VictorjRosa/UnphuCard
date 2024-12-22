using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class InfoTarjetum
{
    public int InfoTarjId { get; set; }

    public string? InfoTarjTitular { get; set; }

    public string? InfoTarjNumTarjeta { get; set; }

    public string? InfoTarjUltNumTarjeta { get; set; }

    public DateOnly? InfoTarjFechaExpira { get; set; }

    public string? InfoTarjCvv { get; set; }

    public int? UsuId { get; set; }
}
