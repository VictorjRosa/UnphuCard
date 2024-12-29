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
        public async Task<List<VwRecarga>> GetHistorialRecargaAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<List<VwRecarga>>($"api/MostrarRecarga/{id}");
        }

        public async Task<List<VwRecarga>> GetHistorialRecargaAsync(int id, string metodoPago)
        {
            return await _httpClient.GetFromJsonAsync<List<VwRecarga>>($"api/MostrarRecargaPorMP/{id}/{metodoPago}");
        }
    }
}
