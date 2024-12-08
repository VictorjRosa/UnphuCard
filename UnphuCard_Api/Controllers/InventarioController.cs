using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard.Controllers
{
    public class InventarioController : Controller
    {
        private readonly UnphuCardContext _context;
        public InventarioController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/ObtenerInvEstablecimiento/{id}")]
        public async Task<ActionResult<VwInventarioEstablecimiento>> GetInvEstablecimiento(int id)
        {
            var establecimiento = await _context.VwInventarioEstablecimientos.FirstOrDefaultAsync(i => i.IdDelEstablecimiento == id);
            if (establecimiento == null)
            {
                return BadRequest("Producto no encontrado");
            }
            return establecimiento;
        }

        [HttpGet("api/ObtenerInvProducto/{id}")]
        public async Task<ActionResult<VwInventarioEstablecimiento>> GetInvProducto(int id)
        {
            var producto = await _context.VwInventarioEstablecimientos.FirstOrDefaultAsync(i => i.IdDelProducto == id);
            if (producto == null)
            {
                return BadRequest("Producto no encontrado");
            }
            return producto;
        }

        [HttpDelete("api/EliminarInventario/{id}")]
        public async Task<IActionResult> DeleteInventario(int id)
        {
            var inventario = await _context.Inventarios.FirstOrDefaultAsync(i => i.InvId == id);
            if (inventario == null)
            {
                return NotFound();
            }
            _context.Inventarios.Remove(inventario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("api/Registrarinventario")]
        public async Task<ActionResult> PostInventario([FromBody] InsertInventario insertInventario)
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
                var inventario = new Inventario()
                {
                    InvCantidad = insertInventario.InvCantidad,
                    InvFecha = fechaEnRD,
                    EstId = insertInventario.EstId,
                    ProdId = insertInventario.ProdId,
                };
                _context.Inventarios.Add(inventario);
                await _context.SaveChangesAsync();

                return Ok(new { id = inventario.InvId, inventario});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
            
        [HttpPut("api/EditarInventario")]
        public async Task<IActionResult> PutInventario(int id, [FromBody] UpdateInventario updateInventario)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                var inventario = await _context.Inventarios.FirstOrDefaultAsync(i => i.InvId == id);
                if (inventario == null)
                {
                    return NotFound("Inventario no encontrado");
                }
                if (updateInventario.InvCantidad.HasValue && updateInventario.InvCantidad != 0)
                {
                    inventario.InvCantidad = updateInventario.InvCantidad;
                }
                inventario.InvFecha = fechaEnRD;
                if (updateInventario.EstId.HasValue && updateInventario.EstId.Value > 0)
                {
                    inventario.EstId = updateInventario.EstId;
                }
                if (updateInventario.ProdId.HasValue && updateInventario.ProdId.Value > 0)
                {
                    inventario.ProdId = updateInventario.ProdId;
                }

                _context.Entry(inventario).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(inventario);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (InventarioExists(id))
                {
                    return NotFound("Inventario no encontrado");
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
        private bool InventarioExists(int id)
        {
            return _context.Inventarios.Any(i => i.InvId == id);
        }
    }
}