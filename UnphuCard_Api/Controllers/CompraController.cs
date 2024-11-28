using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard.Controllers
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
                var compra = new Compra
                {
                    CompMonto = insertCompra.CompMonto,
                    CompFecha = insertCompra.CompFecha,
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
