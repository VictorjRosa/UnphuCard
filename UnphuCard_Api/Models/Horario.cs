using System;
using System.Collections.Generic;

namespace UnphuCard_Api.Models;

public partial class Horario
{
    public int HorId { get; set; }

    public string? HorDia { get; set; }

    public TimeOnly? HorHoraInicio { get; set; }

    public TimeOnly? HorHoraFin { get; set; }

    public int? MatId { get; set; }

    public int? AulaId { get; set; }

    public int? UsuId { get; set; }
}
