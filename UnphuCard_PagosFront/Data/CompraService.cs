using Blazored.LocalStorage;
using System.Net.Http;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;
using UnphuCard_PagosFront.Pages;

namespace UnphuCard_PagosFront.Data
{
    public class CompraService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        public CompraService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;


        }
        public async Task<bool> RegistrarDetalleCompra(InsertDetalleCompra insertDetalleCompra)
        {
            var response = await _httpClient.PostAsJsonAsync("api/RegistrarDetalleCompra", insertDetalleCompra);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(errorMessage);
            }

        }

        public async Task<bool> EnviarCorreo(int? UsuCodigo, int? SesionId, int CompId)
        {
            var response = await _httpClient.GetAsync($"api/MandarCorreoCompra/{UsuCodigo}/{SesionId}/{CompId}");
            return true;
        }

        public async Task<int> ObtenerInvCantidad(int prodId, int estId)
        {
            var compId = await _httpClient.GetFromJsonAsync<int>($"api/ObtenerInvCantidad/{prodId}/{estId}");

            if (compId != 0)
            {
                return compId;
            }
            else
            {
                var errorMessage = "La cantidad es mayor a la que existe";
                throw new ApplicationException(errorMessage);
            }

        }

        public async Task<int> Pagar(InsertCompra insertCompra)
        {
            var response = await _httpClient.PostAsJsonAsync("api/PagarCompra", insertCompra);

            if (response.IsSuccessStatusCode)
            {
                var compId = await response.Content.ReadFromJsonAsync<int>();
                return compId;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(errorMessage);
            }

        }

        public async Task<List<VwInventarioEstablecimiento>> GetEstablecimientoporCategoria(int idProducto)
        {
            return await _httpClient.GetFromJsonAsync<List<VwInventarioEstablecimiento>>($"api/Inventario/DisponibilidadOtrasCafeterias/{idProducto}");
        }
    }
}
