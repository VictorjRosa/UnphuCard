using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.Models;

namespace UnphuCard_Api.Controllers
{
    public class EstablecimientoController : Controller
    {
        private readonly UnphuCardContext _context;
        public EstablecimientoController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/MostrarEstablecimientos")]
        public async Task<ActionResult<IEnumerable<Establecimiento>>> GetEstablecimientos()
        {
            return await _context.Establecimientos.ToListAsync();
        }

        [HttpGet("api/MostrarEstablecimiento/{id}")]
        public async Task<ActionResult<Establecimiento>> GetEstablecimiento(int id)
        {
            var establecimiento = await _context.Establecimientos.FirstOrDefaultAsync(e => e.EstId == id);
            if (establecimiento == null)
            {
                return BadRequest("Establecimiento no encontrado");
            }
            return establecimiento;
        }

        [HttpGet("api/MostrarNombreEst/{id}")]
        public async Task<ActionResult<Establecimiento>> GetNombreEst(int id)
        {
            var estId = await _context.Usuarios.Where(u => u.UsuId == id).Select(u => u.EstId).FirstOrDefaultAsync();
            var estNombre = await _context.Establecimientos.Where(e => e.EstId == estId).Select(e => e.EstDescripcion).FirstOrDefaultAsync();
            if (estNombre == null)
            {
                return BadRequest("Establecimiento no encontrado");
            }
            return Ok(estNombre);
        }
    }
}
