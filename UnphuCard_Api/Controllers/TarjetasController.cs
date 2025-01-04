using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;
using UnphuCard_Api.Service;

namespace UnphuCard_Api.Controllers
{
    public class TarjetasController : Controller
    {
        private readonly UnphuCardContext _context;
        private readonly IServicioEmail _emailService;

        public TarjetasController(UnphuCardContext context, IServicioEmail emailService)
        {
            _context = context;
            _emailService = emailService;
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
            var tarjeta = await _context.Tarjetas.FirstOrDefaultAsync(t => t.TarjId == id);
            if (tarjeta == null)
            {
                return BadRequest("Tarjeta no encontrada");
            }
            return tarjeta;
        }

        [HttpGet("api/ObtenertarjetaProv/{id}")]
        public async Task<ActionResult<TarjetasProvisionale>> GetTarjetaProv(int id)
        {
            var tarjetaProv = await _context.TarjetasProvisionales.FirstOrDefaultAsync(tp => tp.TarjProvId == id);
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

        [HttpPut("api/DesactivarTarjetaProv/{id}")]
        public async Task<IActionResult> DesactivarTarjetaProv(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Tarjeta provisional no válida");
            }
            try
            {
                var revision = await _context.TarjetasProvisionales.Where(tp => tp.TarjProvId == id).Select(tp => tp.UsuId).FirstOrDefaultAsync();
                if (revision == null)
                {
                    return BadRequest("El usuario no tiene una tarjeta provisional asignada");
                }
                var tarjeta = await _context.TarjetasProvisionales.FirstOrDefaultAsync(t => t.TarjProvId == id);
                if (tarjeta == null)
                {
                    return NotFound("Tarjeta provisional no encontrada");
                }
                tarjeta.UsuId = null;
                tarjeta.TarjProvFechaExpiracion = null;
                tarjeta.StatusId = 4;

                _context.Entry(tarjeta).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(tarjeta);
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

        [HttpPut("api/ActivarTarjetaProv/{id}")]
        public async Task<IActionResult> ActivarTarjetaProv(int id, [FromBody] UpdateTarjetaProv updateTarjetaProv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Tarjeta provisional no válida");
            }
            try
            {
                var usuario = await _context.Usuarios.Where(u => u.UsuDocIdentidad == updateTarjetaProv.UsuDocIdentidad).Select(u => u.UsuId).FirstOrDefaultAsync();
                if (usuario == 0)
                {
                    return BadRequest("Usuario no encontrado");
                }
                var revision = await _context.TarjetasProvisionales.Where(tp => tp.UsuId == usuario).Select(tp => tp.UsuId).FirstOrDefaultAsync();
                if (revision != null)
                {
                    return BadRequest("El usuario ya tiene una tarjeta provisional asignada");
                }
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
                tarjetaProv.StatusId = 3;
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

        [HttpGet("api/TarjetaProvNoEntregadas")]
        public async Task<IActionResult> TarjetaProvNoEntregadas()
        {
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                DateTime fechaExpiracion = new(fechaEnRD.Year, fechaEnRD.Month, fechaEnRD.Day, 23, 0, 0);
                var usuarioPendiente = await _context.TarjetasProvisionales
                    .Where(tp => tp.TarjProvFechaExpiracion < fechaEnRD && tp.TarjProvFechaExpiracion.HasValue && tp.StatusId == 3)
                    .Select(tp => new { tp.TarjProvId, tp.UsuId, tp.StatusId, tp.TarjProvFechaExpiracion, tp.TarjProvCodigo })
                    .ToListAsync();
                foreach (var tarjeta in usuarioPendiente)
                {
                    var usuario = await _context.Usuarios
                        .Where(u => u.UsuId == tarjeta.UsuId)
                        .Select(u => new { u.UsuCorreo, u.UsuNombre, u.UsuApellido })
                        .FirstOrDefaultAsync();
                    string mensaje = $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Notificación de Tarjeta No Devuelta - UNPHU</title>
</head>
<body style='margin: 0; padding: 0; background-color: #f4f4f4; font-family: Arial, sans-serif;'>
    <table role='presentation' cellspacing='0' cellpadding='0' border='0' align='center' width='100%' 
           style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; margin-top: 20px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
        
        <!-- Header with Logo -->
        <tr>
            <td style='padding: 40px 0; text-align: center; background-color: #006838; border-radius: 8px 8px 0 0;'>
                <img src='https://fotosunphucard.blob.core.windows.net/fotos/LogoUnphu.png' 
                     alt='UNPHU Logo' style='width: 200px; height: auto;'>
            </td>
        </tr>
        
        <!-- Main Content -->
        <tr>
            <td style='padding: 40px 30px;'>
                <h1 style='color: #006838; margin: 0 0 20px 0; font-size: 24px; font-weight: bold; text-align: center;'>
                    Notificación de Tarjeta No Devuelta
                </h1>
                
                <div style='background-color: #fff8e1; border-left: 4px solid #ffa000; padding: 15px; margin: 0 0 30px 0;'>
                    <p style='color: #666666; font-size: 16px; line-height: 1.5; margin: 0;'>
                        <strong>Atención:</strong> La tarjeta provisional número <span style='color: #006838; font-weight: bold;'>{tarjeta.TarjProvCodigo}</span> 
                        asignada a <span style='color: #006838; font-weight: bold;'>{usuario.UsuNombre} {usuario.UsuApellido}</span> 
                        ha expirado el <span style='color: #006838; font-weight: bold;'>{tarjeta.TarjProvFechaExpiracion:dd/MM/yyyy}</span> y no ha sido devuelta.
                    </p>
                </div>
                
                <p style='color: #555555; font-size: 16px; line-height: 1.5; margin: 0 0 20px 0; text-align: center;'>
                    <strong>Por favor, devuelva la tarjeta a la brevedad posible.</strong>
                </p>                
            </td>
        </tr>
        
        <!-- Footer -->
        <tr>
            <td style='padding: 30px; background-color: #00A650; border-radius: 0 0 8px 8px; text-align: center;'>
                <p style='color: #ffffff; font-size: 14px; margin: 0;'>
                    © {fechaEnRD} Universidad Nacional Pedro Henríquez Ureña
                </p>
                <p style='color: #ffffff; font-size: 12px; margin: 10px 0 0 0;'>
                    Ave. John F. Kennedy Km. 7 1/2, Santo Domingo, República Dominicana
                </p>
            </td>
        </tr>
    </table>
</body>
</html>";

                    await _emailService.SendEmailAsync(usuario.UsuCorreo, "Tarjeta No Devuelta", mensaje);
                    await _emailService.SendEmailAsync("vr19-1028@unphu.edu.do", "Tarjeta No Devuelta", mensaje);
                }
                return Ok("Correos enviados exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("api/ObtenerTarjProv/{statusid}")]
        public async Task<ActionResult<IEnumerable<TarjetasProvisionale>>> GetTarjProvPorEstado(int statusid)
        {
            var estado = await _context.TarjetasProvisionales
                .Where(tp => tp.StatusId == statusid)
                .ToListAsync();

            if (!estado.Any())
            {
                return Ok();
            }

            return Ok(estado);
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
