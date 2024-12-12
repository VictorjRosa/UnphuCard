using Blazored.LocalStorage;
using UnphuCard_Api.DTOS;

namespace UnphuCard_RecargaFront.Data
{
    public class Pago
    {

        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        public Pago(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<bool> PagoRequest(PagoRequest pagoRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/procesar-pago", pagoRequest);

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
