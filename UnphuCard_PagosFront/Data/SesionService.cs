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
                var response = await _httpClient.GetAsync($"api/CheckUserSession?sessionNumber={sessionNumber}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<int?>();
                    return result;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error en la respuesta: {error}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al realizar la solicitud: {ex.Message}");
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

                    if (result != null && result.ContainsKey("sesionToken"))
                    {
                        string sesionToken = result["sesionToken"]?.ToString();

                        if (!string.IsNullOrEmpty(sesionToken))
                         {
                            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "sesionToken", sesionToken);
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

        public async Task<int?> ObtenerSesionPorTokenAsync(string token)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/MostrarSesion/{token}");
                if (response.IsSuccessStatusCode)
                {
                    var sesionId = await response.Content.ReadFromJsonAsync<int>();
                    return sesionId;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al obtener la sesión: {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return null;
            }
        }


    }
}
