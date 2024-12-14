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

        public async Task<List<Producto>> TodoProducto()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Producto>>("api/ObtenerProductos");
            return response ?? new List<Producto>();
        }
        public async Task<List<Producto>> ProductoPorCategoria(int categoriaId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<Producto>>($"api/ObtenerProductosPorCategoria/{categoriaId}");
            return response ?? new List<Producto>();
        }
    }
}
