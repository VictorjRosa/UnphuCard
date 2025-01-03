using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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

    public async Task<bool> RegistrarInventarioConImagen(InsertInventario insertInventario, Stream fileStream, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(insertInventario.ProdDescripcion ?? string.Empty), "ProdDescripcion");
            content.Add(new StringContent(insertInventario.ProdPrecio?.ToString() ?? "0"), "ProdPrecio");
            content.Add(new StringContent(insertInventario.InvCantidad?.ToString() ?? "0"), "InvCantidad");
            content.Add(new StringContent(insertInventario.EstId?.ToString() ?? "0"), "EstId");
            content.Add(new StringContent(insertInventario.StatusId?.ToString() ?? "0"), "StatusId");
            content.Add(new StringContent(insertInventario.CatProdId?.ToString() ?? "0"), "CatProdId");

            content.Add(new StreamContent(fileStream), "foto", fileName);

            var response = await _httpClient.PostAsync("api/Registrarinventario", content);

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
        catch (Exception ex)
        {
            throw new ApplicationException($"Error al registrar el inventario: {ex.Message}");
        }
    }

    public async Task<HttpResponseMessage> EliminarProducto(int id)
    {
        var url = $"api/EliminarInventario/{id}";
        return await _httpClient.DeleteAsync(url);
    }


    public async Task<bool> EditarProducto(int id, UpdateInventario updateInventario)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(updateInventario), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/EditarInventario/{id}", content);

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
        catch (Exception ex)
        {
            throw new ApplicationException($"Error al editar el producto: {ex.Message}");
        }
    }

}

