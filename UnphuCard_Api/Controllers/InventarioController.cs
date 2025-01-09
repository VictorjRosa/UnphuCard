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

        [HttpGet("api/ProductoNormalizado/{productoDescripcion}/{selectedCategory}")]
        public async Task<ActionResult<string>> ProductoNormalizado(string productoDescripcion, int selectedCategory)
        {
            try
            {
                var result = await _productoService.ProductosNormalizados(productoDescripcion, selectedCategory);
                if (string.IsNullOrEmpty(result))
                {
                    return NotFound("Producto no encontrado");
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Producto no encontrado");
            }
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
                var prodId = await _productoService.ValidarProductoExistente(insertInventario.ProdDescripcion);
                var existeProducto = await _context.Inventarios
                    .Where(i => i.ProdId == prodId && i.EstId == insertInventario.EstId)
                    .Select(i => i.InvId)
                    .FirstOrDefaultAsync();
                Producto producto = new Producto();
                Inventario inventario = new Inventario();

                if (prodId == 0 && existeProducto == 0)
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
                else if(prodId != 0 && existeProducto == 0)
                {
                    inventario.InvCantidad = insertInventario.InvCantidad;
                    inventario.InvFecha = fechaEnRD;
                    inventario.EstId = insertInventario.EstId;
                    inventario.ProdId = prodId;
                    _context.Inventarios.Add(inventario);
                    await _context.SaveChangesAsync();
                    return Ok(inventario);
                }
                else
                {
                    return BadRequest("El producto ya existe en el inventario");
                }
                return Ok(new {producto, inventario});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
            
        [HttpPut("api/EditarInventario/{id}")]
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

                var producto = await _context.Productos.FirstOrDefaultAsync(p => p.ProdId == updateInventario.ProdId);
                if (inventario == null)
                {
                    return NotFound("Producto no encontrado");
                }
                if (updateInventario.ProdPrecio.HasValue && updateInventario.ProdPrecio.Value > 0)
                {
                    producto.ProdPrecio = updateInventario.ProdPrecio;
                }
                _context.Entry(producto).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { inventario, producto });

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

        [HttpGet("api/Inventario/DisponibilidadOtrasCafeterias/{idProducto}")]
        public async Task<IActionResult> GetDisponibilidadEnOtrasCafeterias(int idProducto)
        {
            var disponibilidad = await _context.VwInventarioEstablecimientos
                .Where(i => i.IdDelProducto == idProducto && i.CantidadEnElInventario > 0)
                .Select(i => new DisponibilidadModel
                {
                    NombreDelEstablecimiento = i.NombreDelEstablecimiento,
                    CantidadEnElInventario = i.CantidadEnElInventario,
                    IdDelEstablecimiento = i.IdDelEstablecimiento,
                })
                .ToListAsync();
            return Ok(disponibilidad);


        }
        private bool InventarioExists(int id)
        {
            return _context.Inventarios.Any(i => i.InvId == id);
        }
    }
}