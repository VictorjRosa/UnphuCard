using System.Net.Http.Json;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

namespace UnphuCard_RecargaFront.Data
{
  

    public class UsuarioService
    {
        private readonly HttpClient _httpClient;

        public UsuarioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Usuario> GetStudentInfoAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/MostrarUsuario/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Usuario>();
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Error al obtener información del estudiante: {errorMessage}");
            }
        }

        public async Task<string?> GetInfoByMatriculaAsync(string matricula)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Usuario>($"api/MostrarUsuNombreConNombre/{matricula}");
                var usuNombre = $"{response?.UsuNombre} {response?.UsuApellido}";
                return usuNombre;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

    }

}
