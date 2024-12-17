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
    }
}
