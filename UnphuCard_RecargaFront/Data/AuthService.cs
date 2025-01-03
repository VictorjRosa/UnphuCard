using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;




namespace UnphuCard_RecargaFront.Data
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;


        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        public async Task<int> Login(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                await _localStorage.SetItemAsync("authToken", loginResponse.Access_token);

                return loginResponse.RolId;


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



    }
}
