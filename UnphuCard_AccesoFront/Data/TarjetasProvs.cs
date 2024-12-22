using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard_AccesoFront.Data
{
    public class TarjetasProvs
    {
        private readonly HttpClient _httpClient;

        public TarjetasProvs(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TarjetasProvisionale>> GetTarjetasProvsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<TarjetasProvisionale>>("api/ObtenerTarjetasProvs");
        }

        public async Task<string> ActivarTarjetaProv(int tarjProvId, UpdateTarjetaProv updateTarjetaProv)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/ActivarTarjetaProv/{tarjProvId}", updateTarjetaProv);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return loginResponse.Access_token;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(errorMessage);
            }
        }

        public async Task<bool?> VerificarCedula(string cedula)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<bool>($"api/VerificarCedula/{cedula}");
                return response;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<Usuario?> GetEstadoId(string cedula)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Usuario>($"api/ObtenerEstadoId/{cedula}");
                return response;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

    }
}