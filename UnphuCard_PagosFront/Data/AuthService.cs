using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;
using static System.Net.WebRequestMethods;

namespace UnphuCard_PagosFront.Data
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;


        }

        public async Task<string> Login(LoginModel loginModel)
        {
            loginModel.RolId = 4;
            var response = await _httpClient.PostAsJsonAsync("api/Login", loginModel);

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
        public async Task<int> GetUserIdFromTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrEmpty(token))
            {
                throw new ApplicationException("El token no está disponible.");
            }

            var userIdString = DecodeJwtTokenForUserId(token);

            if (int.TryParse(userIdString, out int userId))
            {
                return userId;
            }

            throw new ApplicationException("El userId en el token no es válido.");
        }


        private string DecodeJwtTokenForUserId(string token)
        {
            var parts = token.Split('.');

            if (parts.Length != 3)
            {
                throw new ApplicationException("Token JWT mal formado.");
            }

            var payload = parts[1];
            var base64Url = payload.Replace('-', '+').Replace('_', '/');

            switch (base64Url.Length % 4)
            {
                case 2: base64Url += "=="; break;
                case 3: base64Url += "="; break;
            }

            var jsonBytes = Convert.FromBase64String(base64Url);
            var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (claims != null && claims.ContainsKey("UserId"))
            {
                return claims["UserId"].ToString();
            }

            throw new ApplicationException("No se encontró el userId en el token.");
        }
        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
        }

       
        public async Task<int?> GetEstIdByUserIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/MostrarNombreEst/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

                    if (result != null && result.ContainsKey("estId") && int.TryParse(result["estId"]?.ToString(), out int estId))
                    {
                        return estId; // Retorna el valor de estId como entero
                    }
                    else
                    {
                        Console.WriteLine("La respuesta no contiene 'estId' válido.");
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine($"Error al obtener EstId: {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetEstIdByUserIdAsync: {ex.Message}");
                return null;
            }
        }

    }
}
