using Android.App;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using Plugin.NFC;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using Javax.Security.Auth;

namespace UnphuCard_Lectores
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Configura la detección de NFC
            if (MainApplication.NfcAdapter != null && MainApplication.NfcAdapter.IsEnabled)
            {
                SetupNfcReader();
            }
            else
            {
                // Mensaje si NFC no está disponible o está desactivado
                Console.WriteLine("NFC no disponible o desactivado.");
            }
        }

        private void SetupNfcReader()
        {
            // Configura el lector NFC para que detecte etiquetas
            MainApplication.NfcAdapter.EnableReaderMode(this, new MyNfcReaderCallback(), NfcReaderFlags.NfcA | NfcReaderFlags.SkipNdefCheck, null);
        }

        // Implementa el lector NFC
        private class MyNfcReaderCallback : Java.Lang.Object, NfcAdapter.IReaderCallback
        {
            public async void OnTagDiscovered(Tag tag)
            {
                var id = BitConverter.ToString(tag.GetId()).Replace("-", "");
                Console.WriteLine($"Tag ID: {id}");

                var isValid = await SendTagIdToApi(id);

                // Usa MainPage.Instance para mostrar la alerta
                if (MainPage.Instance != null)
                {
                    if (isValid)
                    {
                        MainPage.Instance.ShowAlert("Acceso Permitido", "La tarjeta es válida");
                    }
                    else
                    {
                        MainPage.Instance.ShowAlert("Acceso Denegado", "La tarjeta no es válida");
                    }
                }
            }

            private async Task<bool> SendTagIdToApi(string tagId)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        string apiUrl = "https://unphucard.azurewebsites.net/api/ValidarAcceso";
                        var jsonContent = JsonConvert.SerializeObject(new { TarjCodigo = tagId, AulaSensor = "101" });
                        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Acceso validado correctamente.");
                            return true; // Tarjeta válida
                        }
                        else
                        {
                            Console.WriteLine("Error al validar el acceso: " + response.ReasonPhrase);
                            return false; // Tarjeta no válida
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al enviar el ID de la tarjeta a la API: " + ex.Message);
                    return false;
                }
            }
        }
    }
}