using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.Models;

namespace UnphuCard_Api.Controllers
{
    public class EstadoController : Controller
    {
        private readonly UnphuCardContext _context;
        public EstadoController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/MostrarEstados")]
        public async Task<ActionResult<IEnumerable<Estado>>> GetEstados()
        {
            return await _context.Estados.ToListAsync();
        }

        [HttpGet("api/MostrarEstado/{id}")]
        public async Task<ActionResult<Estado>> GetEstado(int id)
        {
            var estado = await _context.Estados.FirstOrDefaultAsync(e => e.StatusId == id);
            if (estado == null)
            {
                return BadRequest("Estado no encontrado");
            }
            return estado;
        }
    }
}
