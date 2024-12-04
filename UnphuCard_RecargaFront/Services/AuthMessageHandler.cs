using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace UnphuCard_RecargaFront.Services
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthMessageHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Obtener el token de local storage
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Si el token existe, lo agregamos a la cabecera Authorization
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Continuar con la solicitud
            return await base.SendAsync(request, cancellationToken);
        }
    }

}
