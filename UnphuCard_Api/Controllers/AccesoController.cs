using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard_Api.Controllers
{
    public class AccesoController : Controller
    {
        private readonly UnphuCardContext _context;
        public AccesoController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/MostrarAccesos")]
        public async Task<ActionResult<IEnumerable<VwAccesosUsuario>>> GetAccesos()
        {
            return await _context.VwAccesosUsuarios.ToListAsync();
        }

        [HttpGet("api/MostrarAcceso/{id}")]
        public async Task<ActionResult<VwAccesosUsuario>> GetAcceso(int id)
        {
            var acceso = await _context.VwAccesosUsuarios.FirstOrDefaultAsync(au => au.Id == id);
            if (acceso == null) 
            {
                return BadRequest("Acceso no encontrado");
            }
            return acceso;
        }

        [HttpPost("api/RegistrarAcceso")]
        public async Task<ActionResult> PostAcceso([FromBody] InsertAcceso insertAcceso)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                var acceso = new Acceso()
                {
                    AccesFecha = fechaEnRD,
                    UsuId = insertAcceso.UsuId,
                    AulaId = insertAcceso.AulaId,
                    StatusId = insertAcceso.StatusId,
                };
                _context.Accesos.Add(acceso);
                await _context.SaveChangesAsync();

                return Ok(new { id = acceso.AccesId, acceso });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("api/ValidarAcceso")]
        public async Task<ActionResult> ValidarAcceso([FromBody] ValidarAcceso validarAcceso)
        {
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                // Convertir la fecha a hora
                TimeOnly horaInicioFinal = TimeOnly.FromDateTime(fechaEnRD);
                var AulaId = await _context.Aulas.Where(a => a.AulaSensor == validarAcceso.AulaSensor).Select(a => a.AulaId).FirstOrDefaultAsync();
                var tarjeta = await _context.Tarjetas.FirstOrDefaultAsync(t => t.TarjCodigo == validarAcceso.TarjCodigo);
                var usuarioTarjeta = await _context.Tarjetas.Where(t => t.TarjCodigo == validarAcceso.TarjCodigo).Select(u => u.UsuId).FirstOrDefaultAsync();
                var inscrito = await _context.Inscripciones.FirstOrDefaultAsync(i => i.UsuId == tarjeta.UsuId && i.StatusId == 5);
                var materiaInscrito = await _context.Inscripciones.Where(i => i.UsuId == usuarioTarjeta).Select(i => i.MatId).FirstOrDefaultAsync();
                var usuarioMatProfesor = await _context.Materias.Where(m => m.MatId == Convert.ToInt16(materiaInscrito)).Select(m => new { m.UsuId, m.MatId }).FirstOrDefaultAsync();
                var usuarioHorProfesor = await _context.Horarios.Where(h => h.AulaId == AulaId && h.MatId == Convert.ToInt16(materiaInscrito)).Select(h => h.UsuId).FirstOrDefaultAsync();
                if (usuarioMatProfesor == null)
                {
                    return BadRequest();
                }
                if (usuarioHorProfesor == null)
                {
                    return BadRequest();
                }
                if (materiaInscrito == null)
                {
                    return BadRequest();
                }
                var horarioAccesoEstu = await _context.Horarios.FirstOrDefaultAsync(h => h.AulaId == AulaId && h.MatId == Convert.ToInt16(materiaInscrito) &&
                                                                        h.HorDia == fechaEnRD.DayOfWeek.ToString() &&
                                                                        h.HorHoraInicio <= horaInicioFinal &&
                                                                        h.HorHoraFin <= horaInicioFinal);
                var horarioAccesoProf = await _context.Horarios.FirstOrDefaultAsync(h => h.AulaId == AulaId && h.MatId == Convert.ToInt16(usuarioMatProfesor.MatId) &&
                                                                        h.HorDia == fechaEnRD.DayOfWeek.ToString() &&
                                                                        h.HorHoraInicio <= horaInicioFinal &&
                                                                        h.HorHoraFin <= horaInicioFinal);
                var profesorAcceso = await _context.Accesos.AnyAsync(a => usuarioMatProfesor.UsuId == usuarioHorProfesor && a.AulaId == AulaId && a.StatusId == 8 && a.AccesFecha == fechaEnRD);

                if (tarjeta != null)
                {
                    if (usuarioTarjeta == 1)
                    {
                        if (tarjeta == null || tarjeta.StatusId == 4)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjeta,
                                AulaId = AulaId,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("Tarjeta deshabilitada o no encontrada.");
                        }

                        if (inscrito == null)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjeta,
                                AulaId = AulaId,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("El estudiante no está inscrito en la materia");
                        }

                        if (horarioAccesoEstu == null)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjeta,
                                AulaId = AulaId,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("Acceso fuera del horario permitido o aula incorrecta.");
                        }

                        if (!profesorAcceso)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjeta,
                                AulaId = AulaId,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("El profesor aún no ha ingresado al aula.");
                        }

                        var accesoAprobado = new InsertAcceso()
                        {
                            AccesFecha = fechaEnRD,
                            UsuId = usuarioTarjeta,
                            AulaId = AulaId,
                            StatusId = 8,
                        };
                        await PostAcceso(accesoAprobado);
                        return Ok("Acceso permitido. Tarjeta Estudiante.");
                    }
                    else if (usuarioTarjeta == 2)
                    {
                        if (tarjeta == null || tarjeta.StatusId == 4)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjeta,
                                AulaId = AulaId,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("Tarjeta deshabilitada o no encontrada.");
                        }

                        if (horarioAccesoProf == null)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjeta,
                                AulaId = AulaId,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("Acceso fuera del horario permitido o aula incorrecta.");
                        }
                        var accesoAprobado = new InsertAcceso()
                        {
                            AccesFecha = fechaEnRD,
                            UsuId = usuarioTarjeta,
                            AulaId = AulaId,
                            StatusId = 8,
                        };
                        await PostAcceso(accesoAprobado);
                        return Ok("Acceso permitido. Tarjeta Profesor.");
                    }
                    else if (usuarioTarjeta == 5)
                    {
                        var accesoAprobado = new InsertAcceso()
                        {
                            AccesFecha = fechaEnRD,
                            UsuId = usuarioTarjeta,
                            AulaId = AulaId,
                            StatusId = 8,
                        };
                        await PostAcceso(accesoAprobado);
                        return Ok("Acceso permitido. Tarjeta Admin.");
                    }
                    var accesoFallido1 = new InsertAcceso()
                    {
                        AccesFecha = fechaEnRD,
                        UsuId = usuarioTarjeta,
                        AulaId = AulaId,
                        StatusId = 8,
                    };
                    await PostAcceso(accesoFallido1);
                    return BadRequest("Tarjeta no reconocida.");
                }
                else if (tarjeta == null)
                {
                    var AulaIdProv = await _context.Aulas.Where(a => a.AulaSensor == validarAcceso.AulaSensor).Select(a => a.AulaId).FirstOrDefaultAsync();
                    var tarjetaProv = await _context.TarjetasProvisionales.FirstOrDefaultAsync(t => t.TarjProvCodigo == validarAcceso.TarjCodigo);
                    var usuarioTarjetaProv = await _context.TarjetasProvisionales.Where(u => u.TarjProvCodigo == validarAcceso.TarjCodigo).Select(u => u.UsuId).FirstOrDefaultAsync();
                    var inscritoProv = await _context.Inscripciones.FirstOrDefaultAsync(i => i.UsuId == tarjetaProv.UsuId && i.StatusId == 5);
                    var materiaInscritoProv = await _context.Inscripciones.Where(i => i.UsuId == usuarioTarjetaProv).Select(i => i.MatId).FirstOrDefaultAsync();
                    var usuarioMatProfesorProv = await _context.Materias.Where(m => m.MatId == Convert.ToInt16(materiaInscritoProv)).Select(m => m.UsuId).FirstOrDefaultAsync();
                    var usuarioHorProfesorProv = await _context.Horarios.Where(h => h.AulaId == AulaIdProv && h.MatId == Convert.ToInt16(materiaInscritoProv)).Select(h => h.UsuId).FirstOrDefaultAsync();
                    var materiaProfesorProv = await _context.Materias.Where(i => i.UsuId == usuarioTarjetaProv).Select(i => i.MatId).FirstOrDefaultAsync();
                    if (usuarioMatProfesorProv == null)
                    {
                        return BadRequest();
                    }
                    if (usuarioHorProfesorProv == null)
                    {
                        return BadRequest();
                    }
                    if (materiaInscritoProv == null)
                    {
                        return BadRequest();
                    }
                    var horarioAccesoEstuProv = await _context.Horarios.FirstOrDefaultAsync(h => h.AulaId == AulaIdProv && h.MatId == Convert.ToInt16(materiaInscritoProv) &&
                                                                            h.HorDia == fechaEnRD.DayOfWeek.ToString() &&
                                                                        h.HorHoraInicio <= horaInicioFinal &&
                                                                        h.HorHoraFin <= horaInicioFinal);
                    var horarioAccesoProfProv = await _context.Horarios.FirstOrDefaultAsync(h => h.AulaId == AulaIdProv && h.MatId == Convert.ToInt16(materiaProfesorProv) &&
                                                                            h.HorDia == fechaEnRD.DayOfWeek.ToString() &&
                                                                        h.HorHoraInicio <= horaInicioFinal &&
                                                                        h.HorHoraFin <= horaInicioFinal);
                    var profesorAccesoProv = await _context.Accesos.AnyAsync(a => usuarioMatProfesorProv == usuarioHorProfesorProv && a.AulaId == AulaIdProv && a.StatusId == 8);

                    if (usuarioTarjetaProv == 1)
                    {
                        if (tarjetaProv == null || tarjetaProv.StatusId == 4)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjetaProv,
                                AulaId = AulaIdProv,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("Tarjeta provisional deshabilitada o no encontrada.");
                        }

                        if (tarjetaProv.TarjProvFechaExpiracion < fechaEnRD)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjetaProv,
                                AulaId = AulaIdProv,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("Tarjeta provisional expirada.");
                        }

                        if (inscritoProv == null)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjetaProv,
                                AulaId = AulaIdProv,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("El estudiante no está inscrito en la materia");
                        }

                        if (horarioAccesoEstuProv == null)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjetaProv,
                                AulaId = AulaIdProv,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("Acceso fuera del horario permitido o aula incorrecta.");
                        }

                        if (!profesorAccesoProv)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjetaProv,
                                AulaId = AulaIdProv,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("El profesor aún no ha ingresado al aula.");
                        }

                        var accesoAprobado = new InsertAcceso()
                        {
                            AccesFecha = fechaEnRD,
                            UsuId = usuarioTarjetaProv,
                            AulaId = AulaIdProv,
                            StatusId = 8,
                        };
                        await PostAcceso(accesoAprobado);
                        return Ok("Acceso permitido. Tarjeta provisional Estudiante.");
                    }
                    else if (usuarioTarjetaProv == 2)
                    {
                        if (tarjetaProv == null || tarjetaProv.StatusId == 4)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjetaProv,
                                AulaId = AulaIdProv,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("Tarjeta provisional deshabilitada o no encontrada.");
                        }

                        if (tarjetaProv.TarjProvFechaExpiracion < fechaEnRD)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjetaProv,
                                AulaId = AulaIdProv,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("Tarjeta provisional expirada.");
                        }

                        if (horarioAccesoProfProv == null)
                        {
                            var accesoFallido = new InsertAcceso()
                            {
                                AccesFecha = fechaEnRD,
                                UsuId = usuarioTarjetaProv,
                                AulaId = AulaIdProv,
                                StatusId = 9,
                            };
                            await PostAcceso(accesoFallido);
                            return BadRequest("Acceso fuera del horario permitido o aula incorrecta.");
                        }
                        var accesoAprobado = new InsertAcceso()
                        {
                            AccesFecha = fechaEnRD,
                            UsuId = usuarioTarjetaProv,
                            AulaId = AulaIdProv,
                            StatusId = 8,
                        };
                        await PostAcceso(accesoAprobado);
                        return Ok("Acceso permitido. Tarjeta provisional Profesor.");
                    }
                    else if (usuarioTarjetaProv == 5)
                    {
                        var accesoAprobado = new InsertAcceso()
                        {
                            AccesFecha = fechaEnRD,
                            UsuId = usuarioTarjetaProv,
                            AulaId = AulaIdProv,
                            StatusId = 8,
                        };
                        await PostAcceso(accesoAprobado);
                        return Ok("Acceso permitido. Tarjeta provisional Admin.");
                    }
                    else
                    {
                        var accesoFallido0 = new InsertAcceso()
                        {
                            AccesFecha = fechaEnRD,
                            UsuId = usuarioTarjetaProv,
                            AulaId = AulaIdProv,
                            StatusId = 8,
                        };
                        await PostAcceso(accesoFallido0);
                        return BadRequest("Usuario no reconocido.");
                    }
                }
                else
                {
                    var accesoFallido1 = new InsertAcceso()
                    {
                        AccesFecha = fechaEnRD,
                        UsuId = usuarioTarjeta,
                        AulaId = AulaId,
                        StatusId = 8,
                    };
                    await PostAcceso(accesoFallido1);
                    return BadRequest("Tarjeta no reconocida.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
