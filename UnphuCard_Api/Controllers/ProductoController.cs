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
        public ProductoController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/ObtenerProductos")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        [HttpGet("api/ObtenerProductos/{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.ProdId == id);
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

            // Guardar la imagen en una carpeta local del servidor
            string uploadsFolder = Path.Combine("Fotos"); // Carpeta donde se guardarán las imágenes
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generar un nombre único para la imagen para evitar colisiones
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + foto.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Guardar la imagen en el servidor
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await foto.CopyToAsync(stream);
            }

            // Guardar el producto en la base de datos con la ruta de la imagen
            try
            {
                var producto = new Producto
                {
                    ProdDescripcion = insertProducto.ProdDescripcion,
                    ProdPrecio = insertProducto.ProdPrecio,
                    ProdImagenes = filePath,  // Guardar la ruta de la imagen
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
                    string uploadsFolder = "Fotos"; // Directorio para guardar las imágenes
                    if (foto.FileName.Contains(".jp") || foto.FileName.Contains(".png") || foto.FileName.Contains(".bmp") || foto.FileName.Contains(".webp"))
                    {
                        string rutaFoto = Path.Combine(uploadsFolder, foto.FileName);

                        // Crear el directorio si no existe
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Guardar la imagen en el servidor
                        using (var stream = new FileStream(rutaFoto, FileMode.Create))
                        {
                            await foto.CopyToAsync(stream);
                        }

                        // Actualizar la propiedad de la imagen del producto con la nueva ruta
                        producto.ProdImagenes = rutaFoto;
                    }
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

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(p => p.ProdId == id);
        }
    }
}