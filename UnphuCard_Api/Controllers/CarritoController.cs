using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;
using UnphuCard_Api.Service;

namespace UnphuCard_Api.Controllers
{
    public class CarritoController : Controller
    {
        private readonly UnphuCardContext _context;
        private readonly IServicioEmail _emailService;
        public CarritoController(UnphuCardContext context, IServicioEmail emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet("api/MostrarCarrito/{SesionToken}")]
        public async Task<ActionResult<VwCarritoCompra>> GetCarrito(string SesionToken)
        {
            var carrito = await _context.VwCarritoCompras.Where(s => s.SesiónToken == SesionToken).OrderByDescending(s => s.FechaDeCompra).FirstOrDefaultAsync();
            if (carrito == null)
            {
                return BadRequest("Carrito no encontrado");
            }
            return carrito;
        }

        [HttpDelete("api/EliminarCarrito/{SesionToken}")]
        public async Task<IActionResult> DeleteCarrito(string SesionToken)
        {
            var SesionId = await _context.Sesions.Where(s => s.SesionToken == SesionToken).Select(s => s.SesionId).FirstOrDefaultAsync();
            var carrito = await _context.Carritos.Where(s => s.SesionId == SesionId).OrderByDescending(s => s.CarFecha).FirstOrDefaultAsync();
            if (carrito == null) 
            {
                return NotFound();
            }
            _context.Carritos.Remove(carrito);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("api/RegistrarCarrito")]
        public async Task<ActionResult> PostCarrito([FromBody] InsertCarrito insertCarrito)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                var SesionId = await _context.Sesions.Where(s => s.SesionToken == insertCarrito.SesionToken).Select(s => s.SesionId).FirstOrDefaultAsync();
                var carrito = new Carrito
                {
                    CarFecha = fechaEnRD,
                    CarCantidad = insertCarrito.CarCantidad,
                    ProdId = insertCarrito.ProdId,
                    SesionId = SesionId,
                };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
                return Ok(new { id = carrito.SesionId, carrito});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPut("api/EditarCarrito/{SesionToken}")]
        public async Task<IActionResult> PutCarrito(string SesionToken, [FromBody] UpdateCarrito updateCarrito)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Carrito no válido");
            }
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                // Obtener el nombre del equipo
                var SesionId = await _context.Sesions.Where(s => s.SesionToken == SesionToken).Select(s => s.SesionId).FirstOrDefaultAsync();
                var carrito = await _context.Carritos.FirstOrDefaultAsync(s => s.SesionId == SesionId);
                if (carrito == null)
                {
                    return NotFound("Carrito no encontrado");
                }
                carrito.CarFecha = fechaEnRD;
                if (updateCarrito.CarCantidad.HasValue && updateCarrito.CarCantidad.Value > 0)
                {
                    carrito.CarCantidad = updateCarrito.CarCantidad;
                }
                if (updateCarrito.ProdId.HasValue && updateCarrito.ProdId.Value > 0)
                {
                    carrito.ProdId = updateCarrito.ProdId;
                }

                _context.Entry(carrito).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(carrito);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (CarritoExists(SesionToken))
                {
                    return NotFound("Carrito no encontrado");
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

        [HttpPost("api/PagarCompra")]
        public async Task<ActionResult> ProcesarPago([FromBody] InsertCompra insertCompra)
        {
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuCodigo == insertCompra.UsuCodigo);
                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                // Verifica si el saldo del usuario es suficiente
                if (usuario.UsuSaldo < insertCompra.CompMonto)
                {
                    return BadRequest("Saldo insuficiente.");
                }

                // Realiza la transacción
                usuario.UsuSaldo -= insertCompra.CompMonto;

                // Guardar la compra en la tabla Compras
                var compra = new Compra
                {
                    CompMonto = insertCompra.CompMonto,
                    CompFecha = fechaEnRD,
                    UsuId = insertCompra.UsuCodigo,
                    EstId = insertCompra.EstId,
                    MetPagId = insertCompra.MetPagId,
                    SesionId = insertCompra.SesionId,
                };
                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                var sesionIdCarrito = await _context.Carritos.Where(s => s.SesionId == compra.SesionId).Select(s => s.SesionId).FirstOrDefaultAsync();
                var tokenCompra = await _context.Sesions.Where(t => t.SesionId == compra.SesionId).Select(t => t.SesionToken).FirstOrDefaultAsync();
                var tokenCarrito = await _context.Sesions.Where(t => t.SesionId == sesionIdCarrito).Select(t => t.SesionToken).FirstOrDefaultAsync();
                var itemsCarrito = await _context.Carritos.Where(c => tokenCarrito == tokenCompra).ToListAsync();
                foreach (var item in itemsCarrito)
                {
                    var precio = await _context.Productos.Where(p => p.ProdId == item.ProdId).Select(p => p.ProdPrecio).FirstOrDefaultAsync();
                    var detalleCompra = new DetallesCompra
                    {
                        DetCompCantidad = item.CarCantidad,
                        DetCompPrecio = item.CarCantidad * precio,
                        CompId = compra.CompId,
                        ProdId = item.ProdId,
                        SesionId = compra.SesionId
                    };
                    _context.DetallesCompras.Add(detalleCompra);
                }
                await _context.SaveChangesAsync();

                _context.Carritos.RemoveRange(itemsCarrito);
                await _context.SaveChangesAsync();
                // Construir el mensaje del correo
                string mensaje = $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <title>Comprobante de Transacción</title>
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
        <h1>Comprobante de Transacción</h1>
    </div>
    <div class=""content"">
        <p><strong>{usuario.UsuNombre + " " + usuario.UsuApellido}</strong></p>
        <p>Matrícula: {usuario.UsuMatricula}</p>
        <p>Número de recibo: {recibo.NumeroRecibo}</p>
        <p>RNC: {usuario.UsuDocIdentidad}</p>
        <p>NCF: {recibo.NCF}</p>
        <p>Fecha Válida: {recibo.FechaValida:dd/MM/yyyy HH:mm:ss}</p>

        <table class=""table"">
            <thead>
                <tr>
                    <th>Servicio</th>
                    <th>Beca</th>
                    <th>Monto</th>
                    <th>Descuento</th>
                    <th>Por Pagar</th>
                    <th>Pendiente</th>
                </tr>
            </thead>
            <tbody>
                {string.Join("", detalles.Select(d => $@"
                <tr>
                    <td>{d.Servicio}</td>
                    <td>RD$ {d.Beca:N2}</td>
                    <td>RD$ {d.Monto:N2}</td>
                    <td>RD$ {d.Descuento:N2}</td>
                    <td>RD$ {d.PorPagar:N2}</td>
                    <td>RD$ {d.Pendiente:N2}</td>
                </tr>"))}
            </tbody>
        </table>

        <p><strong>Tarjeta de crédito:</strong> **** **** **** {recibo.TarjetaUltimos4}</p>
        <p><strong>Cantidad pagada con tarjeta:</strong> RD$ {recibo.CantidadPagada:N2}</p>
        <p class=""total"">Total pagado: RD$ {recibo.TotalPagado:N2}</p>
    </div>
</body>
</html>
";

                // Enviar el correo
                await _emailService.SendEmailAsync(estudiante.Email, "Factura de Compra", mensaje);

                return Ok("Transacción exitosa.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool CarritoExists(string SesionToken)
        {
            return _context.Carritos.Any(c => c.SesionId == (_context.Sesions.Where(s => s.SesionToken == SesionToken).Select(s => s.SesionId).FirstOrDefault()));
        }
    }
}
