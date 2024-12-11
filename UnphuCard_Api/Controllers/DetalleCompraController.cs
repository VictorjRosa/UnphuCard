using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult> PostDetalleCompra([FromBody] DetallesCompra detallesCompra)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var detalles = new DetallesCompra()
                {
                    DetCompCantidad = detallesCompra.DetCompCantidad,
                    DetCompPrecio = detallesCompra.DetCompPrecio,
                    CompId = detallesCompra.CompId,
                    ProdId = detallesCompra.ProdId,
                };

                _context.DetallesCompras.Add(detalles);
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
