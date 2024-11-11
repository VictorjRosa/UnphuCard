using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UnphuCard_Api.Models;

namespace UnphuCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarjetasController : Controller
    {
        private readonly UnphuCardContext _context;

        
        public TarjetasController(UnphuCardContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
            var tarjetas = await _context.Tarjetas.ToListAsync();
            return Ok(tarjetas); 
        }

        
        [HttpGet("CheckCardStatus/{codigo}")]
        public async Task<IActionResult> CheckCardStatus(string codigo)
        {
            
            var tarjeta = await _context.Tarjetas
                .Where(t => t.Codigo == codigo)
                .FirstOrDefaultAsync();

            
            if (tarjeta == null)
            {
                tarjeta = await _context.TarjetasProvisionales
                    .Where(t => t.Codigo == codigo)
                    .FirstOrDefaultAsync();
            }

            
            if (tarjeta == null)
            {
                return NotFound(new { message = "Tarjeta no encontrada" });
            }

            
            return Ok(new { tarjeta.Codigo, tarjeta.Status });
        }

        
        [HttpPut("EnableDisableCard/{codigo}")]
        public async Task<IActionResult> EnableDisableCard(string codigo, [FromBody] bool habilitar)
        {
            
            var tarjeta = await _context.Tarjetas
                .Where(t => t.Codigo == codigo)
                .FirstOrDefaultAsync();

            
            if (tarjeta == null)
            {
                tarjeta = await _context.TarjetasProvisionales
                    .Where(t => t.Codigo == codigo)
                    .FirstOrDefaultAsync();
            }

            
            if (tarjeta == null)
            {
                return NotFound(new { message = "Tarjeta no encontrada" });
            }

            
            tarjeta.Status = habilitar ? "Habilitada" : "Deshabilitada";
            tarjeta.StatusId = habilitar ? 1 : 0;  

            
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Estado de la tarjeta actualizado", tarjeta.Codigo, tarjeta.Status });
        }
    }
}
