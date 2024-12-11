using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard_Api.Controllers
{
    public class DispositivosController : Controller
    {
        private readonly UnphuCardContext _context;
        public DispositivosController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/MostrarDispositivo")]
        public async Task<ActionResult<IEnumerable<Dispositivo>>> GetDispositivos()
        {
            return await _context.Dispositivos.ToListAsync();
        }

        [HttpGet("api/MostrarDispositivo/{DispAndroidId}")]
        public async Task<ActionResult<Dispositivo>> GetDispositivo(string DispAndroidId)
        {
            var dispositivo = await _context.Dispositivos.FirstOrDefaultAsync(d => d.DispAndroidId == DispAndroidId);
            if (dispositivo == null)
            {
                return BadRequest("Dispositivo no encontrado");
            }
            return Ok(dispositivo);
        }

        [HttpPost("api/RegistrarDispositivo")]
        public async Task<ActionResult> RegistrarDispositivo([FromBody] InsertDispositivo insertDispositivo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var dispAndroidId = await _context.Dispositivos.FirstOrDefaultAsync(d => d.DispAndroidId == insertDispositivo.DispAndroidId);
                if (dispAndroidId != null)
                {
                    return BadRequest("El dispositivo ya existe");
                }
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                var dispositivo = new Dispositivo()
                {
                    DispAndroidId = insertDispositivo.DispAndroidId,
                    DispFecha = fechaEnRD,
                    EstId = insertDispositivo.EstId,
                };
                _context.Dispositivos.Add(dispositivo);
                await _context.SaveChangesAsync();
                return Ok(new { id = dispositivo.DispId, dispositivo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPut("api/EditarDispositivo/{id}")]
        public async Task<IActionResult> PutDispositivo(int id, [FromBody] InsertDispositivo insertDispositivo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dispositivo no válido");
            }
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                var dispositivo = await _context.Dispositivos.FirstOrDefaultAsync(p => p.DispId == id);
                if (dispositivo == null)
                {
                    return NotFound("Dispositivo no encontrado");
                }
                if (insertDispositivo.DispAndroidId.Length != 0 && insertDispositivo.DispAndroidId != "string")
                {
                    dispositivo.DispAndroidId = insertDispositivo.DispAndroidId;
                }
                dispositivo.DispFecha = fechaEnRD;
                if (insertDispositivo.EstId.HasValue && insertDispositivo.EstId.Value > 0)
                {
                    dispositivo.EstId = insertDispositivo.EstId;
                }
                _context.Entry(dispositivo).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(dispositivo);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (DispositivoExists(id))
                {
                    return NotFound("Dispositivo no encontrado");
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

        private bool DispositivoExists(int id)
        {
            return _context.Dispositivos.Any(p => p.DispId == id);
        }
    }
}
