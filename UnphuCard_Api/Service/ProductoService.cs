using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.Models;

namespace UnphuCard_Api.Service
{
    public class ProductoService
    {
        private readonly UnphuCardContext _context;

        public ProductoService(UnphuCardContext context)
        {
            _context = context;
        }

        // Método para validar si un producto existe y retornar su nombre o null si no existe
        public async Task<string> ValidarProductoExistente(string productoDescripcion)
        {
            // Normalizamos la descripción del producto

            // Realizamos la consulta en la base de datos y obtenemos el producto
            var productoExistente = await _context.Productos
                .Where(p => p.ProdDescripcion == productoDescripcion)  // Normalizamos las descripciones en C#
                .Select(p => p.ProdDescripcion)  // Solo seleccionamos el nombre del producto
                .FirstOrDefaultAsync();  // Usamos FirstOrDefaultAsync porque la consulta se hace sobre IQueryable
            string descripcionNormalizada = NormalizarDescripcion(productoDescripcion);
            string productoNormalizada = NormalizarDescripcion(productoExistente ?? "skip");
            return productoExistente;    // Si existe, retorna el nombre del producto; si no, retorna null
        }

        // Método para normalizar la descripción del producto
        private string NormalizarDescripcion(string descripcion)
        {
            // Reemplazamos las vocales acentuadas por las vocales sin acento
            descripcion = descripcion.Replace('á', 'a')
                                     .Replace('é', 'e')
                                     .Replace('í', 'i')
                                     .Replace('ó', 'o')
                                     .Replace('ú', 'u')
                                     .Replace('Á', 'A')
                                     .Replace('É', 'E')
                                     .Replace('Í', 'I')
                                     .Replace('Ó', 'O')
                                     .Replace('Ú', 'U');

            return descripcion.ToLower();  // Convertimos a minúsculas para que la comparación sea insensible a mayúsculas
        }
    }
}
