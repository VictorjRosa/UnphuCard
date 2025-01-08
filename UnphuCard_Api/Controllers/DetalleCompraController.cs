using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard_Api.Controllers
{
    public class DetalleCompraController : Controller
    {
        private readonly UnphuCardContext _context;
        public DetalleCompraController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/MostrarDetallesCompras")]
        public async Task<ActionResult<IEnumerable<DetallesCompra>>> GetDetallesCompras()
        {
            return await _context.DetallesCompras.ToListAsync();
        }

        [HttpGet("api/MostrarDetalleCompra/{id}")]
        public async Task<ActionResult<DetallesCompra>> GetDetalleCompra(int id)
        {
            var detalleCompra = await _context.DetallesCompras.FirstOrDefaultAsync(dc => dc.DetCompId == id);
            if (detalleCompra == null)
            {
                return BadRequest("Detalle de la compra no encontrado");
            }
            return detalleCompra;
        }

        [HttpPost("api/RegistrarDetalleCompra")]
        public async Task<ActionResult> PostDetalleCompra([FromBody] InsertDetalleCompra insertDetalleCompra)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var detalles = new DetallesCompra()
                {
                    DetCompCantidad = insertDetalleCompra.DetCompCantidad,
                    DetCompPrecio = insertDetalleCompra.DetCompPrecio,
                    CompId = insertDetalleCompra.CompId,
                    ProdId = insertDetalleCompra.ProdId,
                    SesionId = insertDetalleCompra.SesionId,
                };
                _context.DetallesCompras.Add(detalles);
                await _context.SaveChangesAsync();

                var inventario = await _context.Inventarios
                    .Where(i => i.ProdId == insertDetalleCompra.ProdId && i.EstId == insertDetalleCompra.EstId)
                    .Select(i => i.InvCantidad)
                    .FirstOrDefaultAsync();
                inventario =- insertDetalleCompra.DetCompCantidad;
                _context.Entry(inventario).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { id = detalles.DetCompId, detalles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
