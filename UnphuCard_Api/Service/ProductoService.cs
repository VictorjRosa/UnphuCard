using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        public async Task<int> ValidarProductoExistente(string productoDescripcion)
        {
            // Normalizamos la descripción del producto

            // Realizamos la consulta en la base de datos y obtenemos el producto
            var producto = await _context.Productos.ToListAsync();
            string descripcionNormalizada;
            foreach (var item in producto)
            {
                descripcionNormalizada = NormalizarDescripcion(item.ProdDescripcion ?? "skip");
                if (descripcionNormalizada == productoDescripcion)
                {
                    return item.ProdId;
                }
            }
            return 0;
        }

        public async Task<string> ProductosNormalizados(string productoDescripcion, int selectedCategory)
        {
            // Normalizamos la descripción del producto
            List<Producto> producto = new List<Producto>();
            // Realizamos la consulta en la base de datos y obtenemos el producto
            if (selectedCategory == 1000)
            {
                producto = await _context.Productos.ToListAsync();
            }
            else
            {
                producto = await _context.Productos.Where(p => p.CatProdId == selectedCategory).ToListAsync();
            }
            string descripcionNormalizada;
            foreach (var item in producto)
            {
                descripcionNormalizada = NormalizarDescripcion(item.ProdDescripcion ?? "skip");
                if (descripcionNormalizada.Contains(productoDescripcion, StringComparison.CurrentCultureIgnoreCase))
                {
                    return item.ProdDescripcion ?? "";
                }
            }
            return "";
        }

        // Método para normalizar la descripción del producto
        private static string NormalizarDescripcion(string descripcion)
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
