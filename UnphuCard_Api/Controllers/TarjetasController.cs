using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard_Api.Controllers
{
    public class TarjetasController : Controller
    {
        private readonly UnphuCardContext _context;

        
        public TarjetasController(UnphuCardContext context)
        {
            _context = context;
        }

        
        [HttpGet("api/ObtenerTarjetas")]
        public async Task<ActionResult<IEnumerable<Tarjeta>>> GetTarjetas()
        {
            return await _context.Tarjetas.ToListAsync();
        }

        [HttpGet("api/ObtenerTarjetasProvs")]
        public async Task<ActionResult<IEnumerable<TarjetasProvisionale>>> GetTarjetasProv()
        {
            return await _context.TarjetasProvisionales.ToListAsync();
        }

        [HttpGet("api/Obtenertarjeta/{id}")]
        public async Task<ActionResult<Tarjeta>> GetTarjeta(int id)
        {
            var tarjeta = await _context.Tarjetas.FirstOrDefaultAsync(p => p.TarjId == id);
            if (tarjeta == null)
            {
                return BadRequest("Tarjeta no encontrada");
            }
            return tarjeta;
        }

        [HttpGet("api/ObtenertarjetaProv/{id}")]
        public async Task<ActionResult<TarjetasProvisionale>> GetTarjetaProv(int id)
        {
            var tarjetaProv = await _context.TarjetasProvisionales.FirstOrDefaultAsync(p => p.TarjProvId == id);
            if (tarjetaProv == null)
            {
                return BadRequest("Tarjeta provisional no encontrada");
            }
            return tarjetaProv;
        }

        [HttpPut("api/EditarTarjeta/{id}")]
        public async Task<IActionResult> PutTarjeta(int id, [FromBody] UpdateTarjeta updateTarjeta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Tarjeta no válida");
            }
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                var tarjeta = await _context.Tarjetas.FirstOrDefaultAsync(t => t.TarjId == id);
                if (tarjeta == null)
                {
                    return NotFound("Tarjeta no encontrada");
                }
                tarjeta.TarjFecha = fechaEnRD;
                tarjeta.StatusId = updateTarjeta.StatusId;

                _context.Entry(tarjeta).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(tarjeta);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (TarjetaExists(id))
                {
                    return NotFound("Tarjeta no encontrada");
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

        [HttpPut("api/ActivarTarjetaProv/{id}")]
        public async Task<IActionResult> ActivarTarjetaProv(int id, [FromBody] UpdateTarjetaProv updateTarjetaProv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Tarjeta provisional no válida");
            }
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                DateTime fechaExpiracion = new(fechaEnRD.Year,fechaEnRD.Month,fechaEnRD.Day,23,0,0);
                var tarjetaProv = await _context.TarjetasProvisionales.FirstOrDefaultAsync(t => t.TarjProvId == id);
                if (tarjetaProv == null)
                {
                    return NotFound("Tarjeta provisional no encontrada");
                }
                var usuId = await _context.Usuarios.Where(u => u.UsuDocIdentidad == updateTarjetaProv.UsuDocIdentidad).Select(u => u.UsuId).FirstOrDefaultAsync();
                tarjetaProv.TarjProvFecha = fechaEnRD;
                tarjetaProv.StatusId = updateTarjetaProv.StatusId;
                tarjetaProv.UsuId = usuId;
                tarjetaProv.TarjProvFechaExpiracion = fechaExpiracion; 

                _context.Entry(tarjetaProv).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(tarjetaProv);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (TarjetaProvExists(id))
                {
                    return NotFound("Tarjeta provisional no encontrada");
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

        [HttpPut("api/DesactivarTarjetaProv/{id}")]
        public async Task<IActionResult> DesactivarTarjetaProv(int id, [FromBody] UpdateTarjetaProv updateTarjetaProv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Tarjeta provisional no válida");
            }
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                DateTime fechaExpiracion = new(fechaEnRD.Year, fechaEnRD.Month, fechaEnRD.Day, 23, 0, 0);
                var tarjetaProv = await _context.TarjetasProvisionales.FirstOrDefaultAsync(t => t.TarjProvId == id);
                if (tarjetaProv == null)
                {
                    return NotFound("Tarjeta provisional no encontrada");
                }
                var usuId = await _context.Usuarios.Where(u => u.UsuDocIdentidad == updateTarjetaProv.UsuDocIdentidad).Select(u => u.UsuId).FirstOrDefaultAsync();
                tarjetaProv.TarjProvFecha = fechaEnRD;
                tarjetaProv.StatusId = updateTarjetaProv.StatusId;
                tarjetaProv.UsuId = usuId;
                tarjetaProv.TarjProvFechaExpiracion = fechaExpiracion;

                _context.Entry(tarjetaProv).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(tarjetaProv);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (TarjetaProvExists(id))
                {
                    return NotFound("Tarjeta provisional no encontrada");
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

        private bool TarjetaExists(int id)
        {
            return _context.Tarjetas.Any(p => p.TarjId == id);
        }

        private bool TarjetaProvExists(int id)
        {
            return _context.TarjetasProvisionales.Any(p => p.TarjProvId == id);
        }
    }
}
