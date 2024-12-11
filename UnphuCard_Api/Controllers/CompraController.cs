using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard_Api.Controllers
{
    public class CompraController : Controller
    {
        private readonly UnphuCardContext _context;
        public CompraController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/MostrarCompras")]
        public async Task<ActionResult<IEnumerable<VwComprasUsuario>>> GetCompras()
        {
            return await _context.VwComprasUsuarios.ToListAsync();
        }

        [HttpGet("api/MostrarCompra/{id}")]
        public async Task<ActionResult<VwComprasUsuario>> GetCompra(int id)
        {
            var compra = await _context.VwComprasUsuarios.FirstOrDefaultAsync(c => c.IdCompra == id);
            if (compra == null)
            {
                return BadRequest("Compra no encontrada");
            }
            return compra;
        }
        
        [HttpPost("api/RegistrarCompra")]
        public async Task<ActionResult> PostCompra([FromBody] InsertCompra insertCompra)
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
                var compra = new Compra
                {
                    CompMonto = insertCompra.CompMonto,
                    CompFecha = fechaEnRD,
                    UsuId = insertCompra.UsuCodigo,
                    EstId = insertCompra.EstId,
                    MetPagId = insertCompra.MetPagId,
                };

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                return Ok(new { id = compra.CompId, compra});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
