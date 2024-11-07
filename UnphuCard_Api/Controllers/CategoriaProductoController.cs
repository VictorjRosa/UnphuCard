using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.Models;

namespace UnphuCard.Controllers
{
    public class CategoriaProductoController : Controller
    {
        private readonly UnphuCardContext _context;
        public CategoriaProductoController(UnphuCardContext context)
        {
            _context = context;
        }

        [HttpGet("api/MostrarCategoriasProductos")]
        public async Task<ActionResult<IEnumerable<CategoriaProducto>>> GetCategoriasProductos()
        {
            return await _context.CategoriaProductos.ToListAsync();
        }

        [HttpGet("api/MostrarCategoriaProducto/{id}")]
        public async Task<ActionResult<CategoriaProducto>> GetCategoriaProducto(int id)
        {
            var categoria = await _context.CategoriaProductos.FirstOrDefaultAsync(cp => cp.CatProdId == id);
            if (categoria == null)
            {
                return BadRequest("Categoria del producto no encontrada");
            }
            return categoria;
        }
    }
}
