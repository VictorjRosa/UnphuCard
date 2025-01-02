using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;
using UnphuCard_Api.Service;

namespace UnphuCard_Api.Controllers
{
    public class InventarioController : Controller
    {
        private readonly UnphuCardContext _context;
        private readonly IConfiguration _configuration;
        private readonly ProductoService _productoService;
        public InventarioController(UnphuCardContext context, IConfiguration configuration, ProductoService productoService)
        {
            _context = context;
            _configuration = configuration;
            _productoService = productoService;
        }

        [HttpGet("api/ObtenerInvEstablecimiento/{id}")]
        public async Task<ActionResult<VwInventarioEstablecimiento>> GetInvEstablecimiento(int id)
        {
            var establecimiento = await _context.VwInventarioEstablecimientos.Where(i => i.IdDelEstablecimiento == id).ToListAsync();
            if (establecimiento == null)
            {
                return BadRequest("Producto no encontrado");
            }
            return Ok(establecimiento);
        }

        [HttpGet("api/ObtenerInvProducto/{id}")]
        public async Task<ActionResult<VwInventarioEstablecimiento>> GetInvProducto(int id)
        {
            var producto = await _context.VwInventarioEstablecimientos.FirstOrDefaultAsync(i => i.IdDelProducto == id);
            if (producto == null)
            {
                return BadRequest("Producto no encontrado");
            }
            return producto;
        }

        [HttpDelete("api/EliminarInventario/{id}")]
        public async Task<IActionResult> DeleteInventario(int id)
        {
            var inventario = await _context.Inventarios.FirstOrDefaultAsync(i => i.InvId == id);
            if (inventario == null)
            {
                return NotFound();
            }
            _context.Inventarios.Remove(inventario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("api/Registrarinventario")]
        public async Task<ActionResult> PostInventario([FromForm] InsertInventario insertInventario, IFormFile foto)
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
                Producto producto = new Producto();
                Inventario inventario = new Inventario();
                var nombreProducto = await _productoService.ValidarProductoExistente(insertInventario.ProdDescripcion);

                if (nombreProducto == null)  // Si no se pudo insertar el producto (significa que ya existe)
                {
                    // Verificar si se ha proporcionado una imagen
                    if (foto == null || foto.Length == 0)
                    {
                        return BadRequest("Debe proporcionar una imagen.");
                    }

                    // Configurar el BlobServiceClient
                    string connectionString = _configuration["AzureBlobStorage:ConnectionString"];
                    string containerName = _configuration["AzureBlobStorage:ContainerName"];
                    var blobServiceClient = new BlobServiceClient(connectionString);
                    var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                    // Generar un nombre único para el archivo
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FileName);
                    var blobClient = blobContainerClient.GetBlobClient(uniqueFileName);

                    // Subir la imagen a Azure Blob Storage
                    using (var stream = foto.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true); // El "true" sobrescribe el archivo si ya existe
                    }

                    string imageUrl = blobClient.Uri.ToString(); // Obtener la URL del archivo

                    producto.ProdDescripcion = insertInventario.ProdDescripcion;
                    producto.ProdPrecio = insertInventario.ProdPrecio;
                    producto.ProdImagenes = imageUrl;  // Guardar la URL de la imagen
                    producto.StatusId = insertInventario.StatusId;
                    producto.CatProdId = insertInventario.CatProdId;
                    _context.Productos.Add(producto);
                    await _context.SaveChangesAsync();

                    inventario.InvCantidad = insertInventario.InvCantidad;
                    inventario.InvFecha = fechaEnRD;
                    inventario.EstId = insertInventario.EstId;
                    inventario.ProdId = producto.ProdId;
                    _context.Inventarios.Add(inventario);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var prodId = await _context.Productos
                        .Where(p => p.ProdDescripcion == nombreProducto)
                        .Select(p => p.ProdId)
                        .FirstOrDefaultAsync();

                    inventario.InvCantidad = insertInventario.InvCantidad;
                    inventario.InvFecha = fechaEnRD;
                    inventario.EstId = insertInventario.EstId;
                    inventario.ProdId = prodId;
                    _context.Inventarios.Add(inventario);
                    await _context.SaveChangesAsync();
                }

                return Ok(new {producto, inventario});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
            
        [HttpPut("api/EditarInventario")]
        public async Task<IActionResult> PutInventario(int id, [FromBody] UpdateInventario updateInventario)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            try
            {
                // Obtener la zona horaria de República Dominicana (GMT-4)
                TimeZoneInfo zonaHorariaRD = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
                // Obtener la fecha y hora actual en UTC
                DateTime fechaActualUtc = DateTime.UtcNow;
                // Convertir la fecha a la zona horaria de República Dominicana
                DateTime fechaEnRD = TimeZoneInfo.ConvertTimeFromUtc(fechaActualUtc, zonaHorariaRD);
                var inventario = await _context.Inventarios.FirstOrDefaultAsync(i => i.InvId == id);
                if (inventario == null)
                {
                    return NotFound("Inventario no encontrado");
                }
                if (updateInventario.InvCantidad.HasValue && updateInventario.InvCantidad != 0)
                {
                    inventario.InvCantidad = updateInventario.InvCantidad;
                }
                inventario.InvFecha = fechaEnRD;
                if (updateInventario.EstId.HasValue && updateInventario.EstId.Value > 0)
                {
                    inventario.EstId = updateInventario.EstId;
                }
                if (updateInventario.ProdId.HasValue && updateInventario.ProdId.Value > 0)
                {
                    inventario.ProdId = updateInventario.ProdId;
                }

                _context.Entry(inventario).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(inventario);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (InventarioExists(id))
                {
                    return NotFound("Inventario no encontrado");
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
        private bool InventarioExists(int id)
        {
            return _context.Inventarios.Any(i => i.InvId == id);
        }
    }
}