using Blazored.LocalStorage;
using System.Net.Http;
using UnphuCard_Api.DTOS;

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

        public async Task<bool> Pagar(InsertCompra insertCompra)
        {
            var response = await _httpClient.PostAsJsonAsync("api/PagarCompra", insertCompra);

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
    }
}
