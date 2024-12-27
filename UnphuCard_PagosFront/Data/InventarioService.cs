using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using UnphuCard_Api.Models;

public class InventarioService
{
    private readonly HttpClient _httpClient;

    public InventarioService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<VwInventarioEstablecimiento?> GetInventarioEstablecimientoAsync(int id)
    {
        try
        {
            var url = $"api/ObtenerInvEstablecimiento/{id}";
            return await _httpClient.GetFromJsonAsync<VwInventarioEstablecimiento>(url);
        }
        catch (HttpRequestException ex)
        {
            throw new ApplicationException($"Error al obtener el inventario: {ex.Message}", ex);
        }
    }
}

