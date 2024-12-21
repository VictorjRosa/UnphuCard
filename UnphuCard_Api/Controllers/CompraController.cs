﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;
using UnphuCard_Api.Service;

namespace UnphuCard_Api.Controllers
{
    public class CompraController : Controller
    {
        private readonly UnphuCardContext _context;
        private readonly IServicioEmail _emailService;
        public CompraController(UnphuCardContext context, IServicioEmail emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet("api/MostrarCompras")]
        public async Task<ActionResult<IEnumerable<VwComprasUsuario>>> GetCompras()
        {
            return await _context.VwComprasUsuarios.ToListAsync();
        }

        [HttpGet("api/MostrarCompra/{id}")]
        public async Task<ActionResult<VwComprasUsuario>> GetCompra(int id)
        {
            var compra = await _context.VwComprasUsuarios.FirstOrDefaultAsync(c => c.IdCompra == id);
            if (compra == null)
            {
                return BadRequest("Compra no encontrada");
            }
            return compra;
        }
        
        [HttpPost("api/RegistrarCompra")]
        public async Task<ActionResult> PostCompra([FromBody] InsertCompra insertCompra)
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
                var compra = new Compra
                {
                    CompMonto = insertCompra.CompMonto,
                    CompFecha = fechaEnRD,
                    UsuId = insertCompra.UsuCodigo,
                    EstId = insertCompra.EstId,
                    MetPagId = insertCompra.MetPagId,
                };

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                return Ok(new { id = compra.CompId, compra});
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
                var sesionIdDetalleProd = await _context.DetallesCompras.Where(dc => dc.SesionId == insertCompra.SesionId).Select(s => s.SesionId).FirstOrDefaultAsync();
                if (sesionIdDetalleProd == null)
                {
                    return NotFound("Sesión del detalle de la compra no encontrada.");
                }
                var tokenCompra = await _context.Sesions.Where(s => s.SesionId == insertCompra.SesionId).Select(t => t.SesionToken).FirstOrDefaultAsync();
                if (tokenCompra == null)
                {
                    return NotFound("Token de la sesión de la compra no encontrado.");
                }
                var tokenDetalleProd = await _context.Sesions.Where(s => s.SesionId == sesionIdDetalleProd).Select(t => t.SesionToken).FirstOrDefaultAsync();
                if (tokenDetalleProd == null)
                {
                    return NotFound("Token de la sesión del carrito no encontrado.");
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

                var itemsDetalleProd = await _context.DetallesCompras.Where(c => tokenDetalleProd == tokenCompra).ToListAsync();
                List<InfoCarritoProducto> infoProducto = new List<InfoCarritoProducto>();
                foreach (var item in itemsDetalleProd)
                {
                    var producto = await _context.Productos.Where(p => p.ProdId == item.ProdId).Select(p => new { p.ProdDescripcion, p.ProdPrecio, item.DetCompCantidad }).FirstOrDefaultAsync();
                    infoProducto.Add(new InfoCarritoProducto { ProdDescripcion = producto.ProdDescripcion, ProdPrecio = producto.ProdPrecio, ProdCantidad = producto.DetCompCantidad });
                }
                List<InfoCarritoProducto> infoProductoNoDuplicados = infoProducto.Distinct().ToList();
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
        <p>Número de recibo: {insertCompra.CompId}</p>
        <p>RNC: {usuario.UsuDocIdentidad}</p>
        <p>Fecha Válida: {compra.CompFecha:dd/MM/yyyy HH:mm:ss}</p>

        <table class=""table"">
            <thead>
                <tr>
                    <th>Servicio</th>
                    <th>Monto</th>
                    <th>Cantidad</th>
                    <th>Por Pagar</th>
                </tr>
            </thead>
            <tbody>
                {string.Join("", infoProductoNoDuplicados.Select(d => $@"
                <tr>
                    <td>{d.ProdDescripcion}</td>
                    <td>RD$ {d.ProdPrecio:N2}</td>
                    <td>{d.ProdCantidad}</td>
                    <td>RD$ {d.ProdPrecio * d.ProdCantidad:N2}</td>
                </tr>"))}
            </tbody>
        </table>

        <p class=""total"">Total pagado: RD$ {insertCompra.CompMonto:N2}</p>
    </div>
</body>
</html>
";

                // Enviar el correo
                await _emailService.SendEmailAsync(usuario.UsuCorreo, "Factura de Compra", mensaje);

                return Ok("Transacción exitosa.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
