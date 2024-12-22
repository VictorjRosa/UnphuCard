using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard_Api.Controllers
{
    public class ProductoController : Controller
    {
        private readonly UnphuCardContext _context;
        private readonly IConfiguration _configuration;
        public ProductoController(UnphuCardContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("api/ObtenerProductos")]
        public async Task<ActionResult<IEnumerable<VwProducto>>> GetProductos()
        {
            return await _context.VwProductos.ToListAsync();
        }

        [HttpGet("api/ObtenerProductos/{id}")]
        public async Task<ActionResult<VwProducto>> GetProducto(int id)
        {
            var producto = await _context.VwProductos.FirstOrDefaultAsync(p => p.IdDelProducto == id);
            if (producto == null)
            {
                return BadRequest("Producto no encontrado");
            }
            return producto;
        }

        [HttpDelete("api/EliminarProducto/{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.ProdId == id);
            if (producto == null)
            {
                return NotFound();
            }
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("api/RegistrarProducto")]
        public async Task<ActionResult> PostProducto([FromForm] InsertProducto insertProducto, IFormFile foto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

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

            // Guardar el producto en la base de datos con la URL de la imagen
            try
            {
                var producto = new Producto
                {
                    ProdDescripcion = insertProducto.ProdDescripcion,
                    ProdPrecio = insertProducto.ProdPrecio,
                    ProdImagenes = imageUrl,  // Guardar la URL de la imagen
                    StatusId = insertProducto.StatusId,
                    CatProdId = insertProducto.CatProdId
                };
                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                return Ok(new { id = producto.ProdId, producto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPut("api/EditarProducto/{id}")]
        public async Task<IActionResult> PutProducto(int id, [FromForm] UpdateProducto updateProducto, IFormFile? foto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Producto no válido");
            }
            try
            {
                var producto = await _context.Productos.FirstOrDefaultAsync(p => p.ProdId == id);
                if (producto == null)
                {
                    return NotFound("Producto no encontrado");
                }

                if (updateProducto.ProdDescripcion.Length != 0 && updateProducto.ProdDescripcion != "string")
                {
                    producto.ProdDescripcion = updateProducto.ProdDescripcion;
                }

                // Actualizar el precio solo si el valor es mayor a 0
                if (updateProducto.ProdPrecio.HasValue && updateProducto.ProdPrecio.Value > 0)
                {
                    producto.ProdPrecio = updateProducto.ProdPrecio.Value;
                }

                // Actualizar el estado solo si el valor es válido
                if (updateProducto.StatusId.HasValue && updateProducto.StatusId.Value > 0)
                {
                    producto.StatusId = updateProducto.StatusId.Value;
                }

                // Procesar la nueva imagen si se proporciona
                if (foto != null && foto.Length > 0)
                {
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

                    // Actualizar la propiedad de la imagen del producto con la nueva URL
                    producto.ProdImagenes = imageUrl;
                }

                // Actualizar el producto en la base de datos
                _context.Entry(producto).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(producto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (ProductoExists(id))
                {
                    return NotFound("Producto no encontrado");
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

        [HttpGet("api/ObtenerProductosPorCategoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<VwProducto>>> GetProductosPorCategoria(int categoriaId)
        {
            var productos = await _context.VwProductos
                .Where(p => p.IdDeLaCategoríaDelProducto == categoriaId)
                .ToListAsync();

            if (!productos.Any())
            {
                return NotFound("No se encontraron productos para esta categoría.");
            }

            return Ok(productos);
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(p => p.ProdId == id);
        }
    }
}