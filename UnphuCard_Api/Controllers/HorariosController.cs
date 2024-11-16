using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UnphuCard_Api.Models;

namespace UnphuCard.Controllers
{
    public class HorariosController : ControllerBase
    {
        private readonly UnphuCardContext _context;


        public HorariosController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/ObtenerHorarios")]
        public async Task<ActionResult<IEnumerable<Horario>>> GetHorarios()
        {
            return await _context.Horarios.ToListAsync();
        }

        [HttpGet("api/ObtenerHorario/{id}")]
        public async Task<ActionResult<Horario>> GetHorario(int id)
        {
            var horario = await _context.Horarios.FirstOrDefaultAsync(p => p.HorId == id);
            if (horario == null)
            {
                return BadRequest("Horario no encontrado");
            }
            return horario;
        }
    }
}
