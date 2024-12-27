using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;

public class InventarioService
{
    private readonly HttpClient _httpClient;

    public InventarioService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

  
    public async Task<List<VwInventarioEstablecimiento>> GetInventarioEstablecimientoAsync(int id)
    {
        var response = await _httpClient.GetFromJsonAsync<List<VwInventarioEstablecimiento>>($"api/ObtenerInvEstablecimiento/{id}");
        return response ?? new List<VwInventarioEstablecimiento>();
    }

    public async Task<bool?> RegistrarInventario(InsertInventario insertInventario)
    {
        
            var response = await _httpClient.PostAsJsonAsync("api/Registrarinventario", insertInventario);

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

