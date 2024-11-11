using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UnphuCard_Api.Models;

namespace UnphuCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorariosController : ControllerBase
    {
        private readonly UnphuCardContext _context;

        
        public HorariosController(UnphuCardContext context)
        {
            _context = context;
        }

        
        [HttpGet("CheckClassSchedule")]
        public async Task<IActionResult> CheckClassSchedule([FromQuery] string aula, [FromQuery] DateTime fechaHora, [FromQuery] int? estudianteId, [FromQuery] int? profesorId)
        {
            
            if (!estudianteId.HasValue && !profesorId.HasValue)
            {
                return BadRequest(new { message = "Debe especificar el ID de un estudiante o un profesor." });
            }

            
            var horario = await _context.Horarios
                .Where(h => h.Aula == aula && h.FechaHora == fechaHora)
                .Where(h => (estudianteId.HasValue && h.EstudianteId == estudianteId.Value) || (profesorId.HasValue && h.ProfesorId == profesorId.Value))
                .FirstOrDefaultAsync();

            
            if (horario == null)
            {
                return NotFound(new { message = "No se encontró clase para el estudiante/profesor en el aula y horario especificado." });
            }

            
            return Ok(new
            {
                horario.Materia,
                horario.EstudianteId,
                horario.ProfesorId,
                horario.Aula,
                horario.FechaHora
            });
        }
    }
}
