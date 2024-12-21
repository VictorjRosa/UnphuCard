using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard_Api.Controllers
{
    public class SesionController : Controller
    {
        private readonly UnphuCardContext _context;
        public SesionController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/MostrarSesion/{id}")]
        public async Task<ActionResult<Sesion>> GetSesion(int id)
        {
            var sesion = await _context.Sesions.Where(s => s.UsuId == id).OrderByDescending(s => s.SesionFecha).FirstOrDefaultAsync();
            if (sesion == null)
            {
                return BadRequest("Sesión no encontrada");
            }
            return sesion;
        }

        [HttpGet("api/CheckUserSession")]
        public async Task<ActionResult<int?>> CheckUserSession(string sessionNumber)
        {
            try
            {
                var session = await _context.Sesions
                                            .Where(s => s.SesionToken == sessionNumber)
                                            .FirstOrDefaultAsync();

                if (session != null && session.UsuId != 0) 
                {
                    return Ok(session.UsuId);  
                }
                else
                {
                    return NotFound("User not found or session is not valid.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    

    [HttpPut("api/EditarSesion/{estId}")]
        public async Task<IActionResult> PutSesion(int estId, [FromBody] UpdateSesion updateSesion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Sesión no válida");
            }
            try
            {
                var sesion = await _context.Sesions.Where(s => s.EstId == estId).OrderByDescending(s => s.SesionFecha).FirstOrDefaultAsync();
                if (sesion == null)
                {
                    return NotFound("Sesión no encontrada.");
                }
                var usuarioId = await _context.Usuarios.Where(u => u.UsuCodigo == updateSesion.UsuCodigo).Select(u => u.UsuId).FirstOrDefaultAsync();
                sesion.UsuId = usuarioId;
                await _context.SaveChangesAsync();
                return Ok("Sesión actualizada.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (UsuarioExists(updateSesion.UsuCodigo))
                {
                    return NotFound("Usuario no encontrado");
                }
                else if (EstablecimientoExists(estId))
                {
                    return NotFound("Establecimiento no encontrado");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("api/RegistrarSesion")]
        public async Task<ActionResult> PostSesion([FromBody] int estId)
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
                // Obtener el nombre del equipo

                var sesion = new Sesion()
                {
                    SesionToken = Guid.NewGuid().ToString(),
                    SesionFecha = fechaEnRD,
                    EstId = estId,
                };
                _context.Sesions.Add(sesion);
                await _context.SaveChangesAsync();
                return Ok(new { id = sesion.SesionToken, sesion });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Sesions.Any(c => c.UsuId == id);
        }
        private bool EstablecimientoExists(int id)
        {
            return _context.Sesions.Any(c => c.EstId == id);
        }
    }
}
