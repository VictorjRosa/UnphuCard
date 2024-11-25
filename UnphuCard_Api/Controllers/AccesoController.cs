using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard.DTOS;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard.Controllers
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
                var acceso = new Acceso()
                {
                    AccesFecha = insertAcceso.AccesFecha,
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
                var AulaId = await _context.Aulas.Where(a => a.AulaSensor == validarAcceso.AulaSensor).Select(a => a.AulaId).FirstOrDefaultAsync();
                var tarjeta = await _context.Tarjetas.FirstOrDefaultAsync(t => t.TarjCodigo == validarAcceso.TarjCodigo);
                if (tarjeta != null)
                {
                    var usuarioTarjeta = await _context.Tarjetas.Where(t => t.TarjCodigo == validarAcceso.TarjCodigo).Select(u => u.UsuId).FirstOrDefaultAsync();
                    var inscrito = await _context.Inscripciones.FirstOrDefaultAsync(i => i.UsuId == tarjeta.UsuId && i.StatusId == 5);
                    var materiaInscrito = await _context.Inscripciones.Where(i => i.UsuId == usuarioTarjeta).Select(i => i.MatId).FirstOrDefaultAsync();
                    var usuarioMatProfesor = await _context.Materias.Where(m => m.MatId == Convert.ToInt16(materiaInscrito)).Select(m => m.UsuId).FirstOrDefaultAsync();
                    var usuarioHorProfesor = await _context.Horarios.Where(h => h.AulaId == AulaId && h.MatId == Convert.ToInt16(materiaInscrito)).Select(h => h.UsuId).FirstOrDefaultAsync();
                    var materiaProfesor = await _context.Materias.Where(i => i.UsuId == usuarioTarjeta).Select(i => i.MatId).FirstOrDefaultAsync();
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
                    TimeOnly horaInicio = new TimeOnly(15, 45, 00);
                    TimeOnly horaFin = new TimeOnly(17, 45, 00);
                    var horarioAccesoEstu = await _context.Horarios.FirstOrDefaultAsync(h => h.AulaId == AulaId && h.MatId == Convert.ToInt16(materiaInscrito) &&
                                                                            h.HorDia == "Monday"/*fechaEnRD.DayOfWeek.ToString()  */ &&
                                                                            h.HorHoraInicio <= horaInicio/*fechaEnRD.TimeOfDay*/ &&
                                                                            h.HorHoraFin <= horaFin/*fechaEnRD.TimeOfDay*/);
                    var horarioAccesoProf = await _context.Horarios.FirstOrDefaultAsync(h => h.AulaId == AulaId && h.MatId == Convert.ToInt16(materiaProfesor) &&
                                                                            h.HorDia == "Monday"/*fechaEnRD.DayOfWeek.ToString()  */ &&
                                                                            h.HorHoraInicio <= horaInicio/*fechaEnRD.TimeOfDay*/ &&
                                                                            h.HorHoraFin <= horaFin/*fechaEnRD.TimeOfDay*/);
                    var profesorAcceso = await _context.Accesos.AnyAsync(a => usuarioMatProfesor == usuarioHorProfesor && a.AulaId == AulaId && a.StatusId == 8);

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
                }
                else
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
                    TimeOnly horaInicioProv = new TimeOnly(15, 45, 00);
                    TimeOnly horaFinProv = new TimeOnly(17, 45, 00);
                    var horarioAccesoEstuProv = await _context.Horarios.FirstOrDefaultAsync(h => h.AulaId == AulaIdProv && h.MatId == Convert.ToInt16(materiaInscritoProv) &&
                                                                            h.HorDia == "Monday"/*fechaEnRD.DayOfWeek.ToString()  */ &&
                                                                            h.HorHoraInicio <= horaInicioProv/*fechaEnRD.TimeOfDay*/ &&
                                                                            h.HorHoraFin <= horaFinProv/*fechaEnRD.TimeOfDay*/);
                    var horarioAccesoProfProv = await _context.Horarios.FirstOrDefaultAsync(h => h.AulaId == AulaIdProv && h.MatId == Convert.ToInt16(materiaProfesorProv) &&
                                                                            h.HorDia == "Monday"/*fechaEnRD.DayOfWeek.ToString()  */ &&
                                                                            h.HorHoraInicio <= horaInicioProv/*fechaEnRD.TimeOfDay*/ &&
                                                                            h.HorHoraFin <= horaFinProv/*fechaEnRD.TimeOfDay*/);
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
                    }
                    return BadRequest("Usuario no reconocido.");
                }
                return BadRequest("Tarjeta no reconocida.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
