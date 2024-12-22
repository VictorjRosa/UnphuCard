using Blazored.LocalStorage;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace UnphuCard_PagosFront.Data
{
    public class SesionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly IJSRuntime _jsRuntime;

        public SesionService(HttpClient httpClient, ILocalStorageService localStorage, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _jsRuntime = jsRuntime;
        }
        public async Task<int?> SesionUser(string sessionNumber)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<int?>($"api/CheckUserSession?sessionNumber={sessionNumber}");
                return response;
            }
            catch (HttpRequestException)
            {
                return null;
            }

        }


        public async Task<bool> RegistrarSesionAsync(int estId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/RegistrarSesion",estId);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

                    if (result != null && result.ContainsKey("SesionToken"))
                    {
                        string sesionToken = result["SesionToken"]?.ToString();

                        if (!string.IsNullOrEmpty(sesionToken))
                        {
                            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "SesionToken", sesionToken);
                            return true;
                        }
                    }

                    Console.WriteLine("Respuesta inesperada del servidor.");
                    return false;
                }
                else
                {
                    Console.WriteLine($"Error al registrar la sesión. Código de estado: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

    
    }
}
