using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnphuCard.DTOS;

public class CardnetService
{
    private readonly HttpClient _httpClient;
    private string _accessToken;
    private DateTime _tokenExpiration;

    public CardnetService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Método para obtener el token de autenticación
    private async Task<string> ObtenerTokenAsync()
    {
        if (_accessToken != null && DateTime.UtcNow < _tokenExpiration)
        {
            return _accessToken; // Usa el token almacenado si no ha expirado
        }

        var urlToken = "https://api.cardnet.com.do/oauth/token";
        var credentials = new
        {
            client_id = "TU_CLIENT_ID",
            client_secret = "TU_CLIENT_SECRET",
            grant_type = "client_credentials"
        };

        var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(urlToken, content);

        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = await response.Content.ReadAsStringAsync();
            var tokenData = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);

            _accessToken = tokenData.access_token;
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenData.expires_in);

            return _accessToken;
        }
        else
        {
            throw new Exception("Error al obtener el token de autenticación");
        }
    }

    // Método para procesar el pago
    public async Task<string> ProcesarPagoAsync(PagoRequest pagoRequest)
    {
        var token = pagoRequest.Token;  // Ahora usamos el token recibido del frontend
        var urlPago = "https://api.cardnet.com.do/v1/pagos";

        var data = new
        {
            token = token,  // Token que se obtuvo de Cardnet
            amount = pagoRequest.Amount,
            currency = "DOP",
            orderNumber = pagoRequest.OrderNumber,
            description = pagoRequest.Description,
            customerEmail = pagoRequest.CustomerEmail
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(urlPago, jsonContent);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();  // Aquí se obtiene la respuesta del pago
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error en la transacción: {error}");
        }
    }

}


