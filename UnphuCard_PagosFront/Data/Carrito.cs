using System.Net.Http;
using UnphuCard_Api.Models;

namespace UnphuCard_PagosFront.Data
{
    public class Carrito
    {
        private readonly HttpClient _httpClient;

        public Carrito(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        
        public async Task<List<CategoriaProducto>> GetCategoriasProductosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<CategoriaProducto>>("api/MostrarCategoriasProductos");
        }

        public async Task<CategoriaProducto?> GetCategoriaProductoByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<CategoriaProducto>($"api/MostrarCategoriaProducto/{id}");
                return response;
            }
            catch (HttpRequestException)
            {
                return null; 
            }
        }
    }
}
