using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnphuCard_QR.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "https://unphucard.azurewebsites.net/api"; // Cambia a tu URL base

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<int?> ObtenerEstablecimientoId(string androidId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/MostrarDispositivo/{androidId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var dispositivo = JsonSerializer.Deserialize<DispositivoResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return dispositivo?.EstId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el ID del establecimiento: {ex.Message}");
                return null;
            }
        }


        private class DispositivoResponse
        {
            public int DispId { get; set; }
            public string? DispNumSerie { get; set; }
            public DateTime DispFecha { get; set; }
            public int EstId { get; set; }
        }
    }
}
