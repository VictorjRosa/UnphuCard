using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UnphuCard_Api.Models;  

namespace UnphuCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscripcionesController : ControllerBase
    {
        private readonly UnphuCardContext _context;

        
        public InscripcionesController(UnphuCardContext context)
        {
            _context = context;
        }

        
        [HttpGet("CheckEnrollmentStatus/{usuId}/{materiaId}")]
        public async Task<IActionResult> CheckEnrollmentStatus(int usuId, int materiaId)
        {
            
            var inscripcion = await _context.Inscripciones
                .Where(i => i.UsuId == usuId && i.MateriaId == materiaId)
                .FirstOrDefaultAsync();

            
            if (inscripcion == null)
            {
                return NotFound(new { message = "Inscripción no encontrada" });
            }

            
            return Ok(new { inscripcion.UsuId, inscripcion.MateriaId, inscripcion.Status });
        }

        
        [HttpPut("UpdateEnrollmentStatus/{usuId}/{materiaId}")]
        public async Task<IActionResult> UpdateEnrollmentStatus(int usuId, int materiaId, [FromBody] int nuevoStatusId)
        {
            
            var inscripcion = await _context.Inscripciones
                .Where(i => i.UsuId == usuId && i.MateriaId == materiaId)
                .FirstOrDefaultAsync();

            
            if (inscripcion == null)
            {
                return NotFound(new { message = "Inscripción no encontrada" });
            }

            
            inscripcion.StatusId = nuevoStatusId;
            inscripcion.Status = nuevoStatusId == 1 ? "Aprobada" : (nuevoStatusId == 2 ? "Retirada" : "Pendiente");

            
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Estado de la inscripción actualizado", inscripcion.UsuId, inscripcion.MateriaId, inscripcion.Status });
        }
    }
}
