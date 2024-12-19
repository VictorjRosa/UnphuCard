using Microsoft.AspNetCore.Mvc;
using UnphuCard_Api.Models;
using UnphuCard_Api.DTOS;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.Service;

namespace UnphuCard_Api.Controllers
{
    public class RecargasController : Controller
    {

        private readonly UnphuCardContext _context;
        private readonly CardnetService _cardnetService;
        private readonly IServicioEmail _emailService;

        public RecargasController(UnphuCardContext context, CardnetService cardnetService, IServicioEmail emailService)
        {
            _context = context;
            _cardnetService = cardnetService;
            _emailService = emailService;
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

                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuId == request.UsuarioId);
                var metodoPago = await _context.MetodoPagos.Where(mp => mp.MetPagId == request.MetodoPago).Select(mp => mp.MetPagDescripcion).FirstOrDefaultAsync();
                string mensaje = $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <title>Comprobante de Recarga</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
        }}
        .header {{
            background-color: #007b3e;
            color: white;
            text-align: center;
            padding: 10px;
        }}
        .header img {{
            max-height: 50px;
        }}
        .content {{
            padding: 20px;
        }}
        .table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}
        .table th, .table td {{
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }}
        .table th {{
            background-color: #f2f2f2;
        }}
        .total {{
            font-size: 1.5em;
            font-weight: bold;
            text-align: center;
            margin-top: 20px;
            color: #007b3e;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <img src=""https://www.unphu.edu.do/images/logo-unphu.png"" alt=""UNPHU"">
        <h1>Comprobante de Recarga</h1>
    </div>
    <div class=""content"">
        <p><strong>{usuario.UsuNombre + " " + usuario.UsuApellido}</strong></p>
        <p>Matrícula: {usuario.UsuMatricula}</p>
        <p>Número de recibo: {request.OrderNumber}</p>
        <p>RNC: {usuario.UsuDocIdentidad}</p>
        <p>Fecha de Recarga: {fechaEnRD:dd/MM/yyyy HH:mm:ss}</p>

        <table class=""table"">
            <thead>
                <tr>
                    <th>Descripción</th>
                    <th>Monto Recargado</th>
                    <th>Método de Pago</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>Recarga de UNPHUCard</td>
                    <td>RD$ {request.Amount:N2}</td>
                    <td>{(metodoPago == "EF" ? "Efectivo" : "Tarjeta")}</td>
                </tr>
            </tbody>
        </table>

        <p class=""total"">Total recargado: RD$ {request.Amount:N2}</p>
    </div>
</body>
</html>
";

                await _emailService.SendEmailAsync(usuario.UsuCorreo, "Comprobante de Recarga - UNPHUCard", mensaje);

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
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuId == usuarioId);

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



