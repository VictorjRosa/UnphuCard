using Azure;
using UnphuCard_Api.Models;

namespace UnphuCard_AccesoFront.Data
{
    public class Estados
    {
        private readonly HttpClient _httpClient;

        public Estados(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetEstadosAsync(int statudId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/MostrarEstado/{statudId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        return "Error: respuesta vacía";
                    }
                    return content;
                }
                else
                {
                    return "Error: No se pudo obtener el estado";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "Error: Excepción durante la solicitud";
            }
        }
    }
}
