using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.Models;

namespace UnphuCard_Api.Controllers
{
    public class MetodoPagoController : Controller
    {
        private readonly UnphuCardContext _context;
        public MetodoPagoController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/MostrarMetodoPago")]
        public async Task<ActionResult<IEnumerable<MetodoPago>>> GetMetodosPago()
        {
            return await _context.MetodoPagos.ToListAsync();
        }

        [HttpGet("api/MostrarMetodoPago/{id}")]
        public async Task<ActionResult<MetodoPago>> GetMetodoPago(int id)
        {
            var metodoPago = await _context.MetodoPagos.FirstOrDefaultAsync(p => p.MetPagId == id);
            if (metodoPago == null)
            {
                return BadRequest("Producto no encontrado");
            }
            return metodoPago;
        }
    }
}
