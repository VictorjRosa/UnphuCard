using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UnphuCard_Lectores
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async Task EnviarIDaAPI(string tagId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var data = new { tarjetaId = tagId, aulaSensor = "101" };
                    var json = JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("https://unphucard.azurewebsites.net/api/ValidarAcceso", content);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Acceso Permitido", "La tarjeta es válida", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Acceso Denegado", "La tarjeta no es válida", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al conectar con la API: {ex.Message}", "OK");
            }
        }

        private async void OnApiTestButtonClicked(object sender, EventArgs e)
        {
            string testTagId = "1"; // Cambia a cualquier ID de prueba
            await EnviarIDaAPI(testTagId);
        }
    }
}
