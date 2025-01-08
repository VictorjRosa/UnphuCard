using Blazored.LocalStorage;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard_PagosFront.Data
{
    public class ProductosService
    {
        private readonly HttpClient _httpClient;
        public ProductosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<VwProducto>> TodoProducto()
        {
            var response = await _httpClient.GetFromJsonAsync<List<VwProducto>>("api/ObtenerProductos");
            return response ?? new List<VwProducto>();
        }
        public async Task<List<VwProducto>> ProductoPorCategoria(int categoriaId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<VwProducto>>($"api/ObtenerProductosPorCategoria/{categoriaId}");
            return response ?? new List<VwProducto>();
        }
        public async Task<string> ProductoNormalizado(string searchTerm, int selectedCategory)
        {
            try
            {
                // Realiza la llamada a la API para obtener el producto normalizado
                var response = await _httpClient.GetAsync($"api/ProductoNormalizado/{searchTerm}/{selectedCategory}");

                // Si la respuesta es exitosa, obtén el contenido
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;  // Retorna el resultado
                }
                else
                {
                    // Si no se encuentra el producto, muestra un mensaje o maneja el error
                    return "Producto no encontrado";
                }
            }
            catch (Exception)
            {
                // Maneja errores de conexión u otros problemas
                return "Producto no encontrado";
            }
        }
    }
}
