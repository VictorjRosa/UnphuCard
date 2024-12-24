namespace UnphuCard_AccesoFront.Data
{
    public class Usuarios
    {
        private readonly HttpClient _httpClient;

        public Usuarios(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetUsuariosNombresAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/MostrarUsuarioNombre/{id}");
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
                    return "Error: No se pudo obtener el nombre";
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
