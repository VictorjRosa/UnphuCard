using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard.DTOS;
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
        public async Task<ActionResult> ValidarAcceso(int tarjetaId, string aulaSensor)
        {
            try
            {
                var AulaId = await _context.Aulas.Where(a => a.AulaSensor == aulaSensor).Select(a => a.AulaId).FirstOrDefaultAsync();

                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);

                var tarjeta = await _context.Tarjetas.FirstOrDefaultAsync(t => t.TarjId == tarjetaId);
                var usuarioTarjeta = await _context.Tarjetas.Where(u => u.TarjId == tarjetaId).Select(u => u.UsuId).FirstOrDefaultAsync();
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

                var inscrito = await _context.Inscripciones.FirstOrDefaultAsync(i => i.UsuId == tarjeta.UsuId && i.StatusId == 5);
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
                    return BadRequest("El usuario no está inscrito en la materia");
                }

                var materiaInscrito = await _context.Inscripciones.Where(i => i.UsuId == usuarioTarjeta).Select(i => i.MatId).FirstOrDefaultAsync();
                if (materiaInscrito == null)
                {
                    return BadRequest();
                }
                TimeOnly horaInicio = new TimeOnly(15, 45, 00);
                TimeOnly horaFin = new TimeOnly(17, 45, 00);
                var horario = await _context.Horarios.FirstOrDefaultAsync(h => h.AulaId == AulaId && h.MatId == Convert.ToInt16(materiaInscrito) &&
                                                                        h.HorDia == "Monday"/*fechaEnRD.DayOfWeek.ToString()  */ &&
                                                                        h.HorHoraInicio <= horaInicio/*fechaEnRD.TimeOfDay*/ &&
                                                                        h.HorHoraFin <= horaFin/*fechaEnRD.TimeOfDay*/);

                if (horario == null)
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

                var materiaProfesor = await _context.Materias.Where(m => m.MatId == Convert.ToInt16(materiaInscrito)).Select(m => m.UsuId).FirstOrDefaultAsync();
                if (materiaProfesor == null)
                {
                    return BadRequest();
                }
                var horarioProfesor = await _context.Horarios.Where(h => h.AulaId == AulaId && h.MatId == Convert.ToInt16(materiaInscrito)).Select(h => h.UsuId).FirstOrDefaultAsync();
                if (horarioProfesor == null)
                {
                    return BadRequest();
                }
                var profesorAcceso = await _context.Accesos.AnyAsync(a => materiaProfesor == horarioProfesor && a.AulaId == AulaId && a.StatusId == 8);
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
                return Ok("Acceso permitido.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
