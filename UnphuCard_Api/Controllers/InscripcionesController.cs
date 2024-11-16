using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UnphuCard_Api.Models;
using UnphuCard_Api.DTOS;

namespace UnphuCard.Controllers
{
    public class InscripcionesController : ControllerBase
    {
        private readonly UnphuCardContext _context;

        
        public InscripcionesController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/ObtenerInscripciones")]
        public async Task<ActionResult<IEnumerable<Inscripcione>>> GetInscripciones()
        {
            return await _context.Inscripciones.ToListAsync();
        }

        [HttpGet("api/ObtenerInscripcion/{id}")]
        public async Task<ActionResult<Inscripcione>> GetInscripcion(int id)
        {
            var inscripcion = await _context.Inscripciones.FirstOrDefaultAsync(p => p.InsId == id);
            if (inscripcion == null)
            {
                return BadRequest("inscripcion no encontrada");
            }
            return inscripcion;
        }

        [HttpPut("api/EditarInscripcion")]
        public async Task<IActionResult> PutInscripcion(int id, [FromBody] UpdateInscripcion updateInscripcion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Inscripción no válida");
            }
            try
            {
                var inscripcion = await _context.Inscripciones.FirstOrDefaultAsync(t => t.InsId == id);
                if (inscripcion == null)
                {
                    return NotFound("Inscripción no encontrada");
                }
                inscripcion.StatusId = updateInscripcion.StatusId;

                _context.Entry(inscripcion).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(inscripcion);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (InscripcionExists(id))
                {
                    return NotFound("Inscripción no encontrada");
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

        private bool InscripcionExists(int id)
        {
            return _context.Inscripciones.Any(p => p.InsId == id);
        }
    }
}
