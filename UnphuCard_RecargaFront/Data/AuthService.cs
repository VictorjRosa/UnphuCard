using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NuGet.Protocol.Plugins;
using UnphuCard.DTOS;
using UnphuCard_Api.DTOS;
using UnphuCard_Api.Models;


namespace UnphuCard_RecargaFront.Data
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
           
        }

        public async Task<string> Login(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return loginResponse.access_token;


            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(errorMessage);
            }

        }



    }
}
