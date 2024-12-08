using Microsoft.AspNetCore.Mvc;
using UnphuCard_Api.Models;
using UnphuCard.DTOS;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace UnphuCard.Controllers
{
    public class RecargasController : Controller
    {

        private readonly UnphuCardContext _context;
        private readonly CardnetService _cardnetService;

        public RecargasController(UnphuCardContext context, CardnetService cardnetService)
        {
            _context = context;
            _cardnetService = cardnetService;   
        }


        [HttpPost("api/procesar-pago")]
        public async Task<IActionResult> ProcesarPago([FromBody] PagoRequest request)
        {
            try
            {
                if (request.Amount <= 0)
                {
                    return BadRequest("El monto debe ser mayor a cero.");
                }

                if (request.MetodoPago == 3)
                {
                    var resultado = await _cardnetService.ProcesarPagoAsync(request);

                    if (string.IsNullOrEmpty(resultado))
                    {
                        return BadRequest("El pago con tarjeta no fue exitoso.");
                    }
                }
                else if (request.MetodoPago == 2)
                {
                    Console.WriteLine("Pago procesado en efectivo.");
                }
                else
                {
                    return BadRequest("Método de pago no válido.");
                }

                var actualizado = await ActualizarSaldoAsync(request.UsuarioId, request.Amount);
                if (!actualizado)
                {
                    return BadRequest("No se pudo actualizar el saldo del usuario.");
                }

                return Ok("Pago procesado y saldo actualizado.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al procesar el pago: {ex.Message}");
            }
        }

        [HttpGet("api/MostrarRecargas")]
        public async Task<ActionResult<IEnumerable<VwRecarga>>> GetRecargas()
        {
            return await _context.VwRecargas.ToListAsync();
        }

        [HttpGet("api/MostrarRecarga/{id}")]
        public async Task<ActionResult<VwRecarga>> GetRecarga(int id)
        {
            var recarga = await _context.VwRecargas.FirstOrDefaultAsync(r => r.IdDelUsuario == id);
            if (recarga == null)
            {
                return BadRequest("Recarga no encontrada");
            }
            return recarga;
        }

        public async Task<bool> ActualizarSaldoAsync(int usuarioId, decimal monto)
        {
            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.UsuId == usuarioId);

            if (usuario == null)
            {
                return false; 
            }

            usuario.UsuSaldo = (usuario.UsuSaldo ?? 0) + monto;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}



