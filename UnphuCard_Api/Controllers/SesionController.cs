using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard.Controllers
{
    public class SesionController : Controller
    {
        private readonly UnphuCardContext _context;
        public SesionController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/MostrarSesion{id}")]
        public async Task<ActionResult<Sesion>> GetSesion(int id)
        {
            var sesion = await _context.Sesions.Where(s => s.UsuId == id).OrderByDescending(s => s.SesionFecha).FirstOrDefaultAsync();
            if (sesion == null)
            {
                return BadRequest("Sesión no encontrada");
            }
            return sesion;
        }

        [HttpPost("api/RegistrarSesion")]
        public async Task<ActionResult> PostSesion([FromBody] InsertSesion insertSesion)
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

                var sesion = new Sesion()
                {
                    SesionToken = Guid.NewGuid().ToString(),
                    SesionFecha = fechaEnRD,
                    UsuId = insertSesion.UsuId,
                };
                _context.Sesions.Add(sesion);
                await _context.SaveChangesAsync();
                return Ok(new { id = sesion.SesionId, sesion });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
