using UnphuCard_Api.Models;

namespace UnphuCard_RecargaFront.Data
{
    public class HistorialRecarga
    {
        private readonly HttpClient _httpClient;

        public HistorialRecarga(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<VwRecarga>> GetHistorialRecargaAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<VwRecarga>>("api/MostrarRecargas");
        }

    }
}
