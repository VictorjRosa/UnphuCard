using Android.App;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
                Console.WriteLine("NFC no disponible o desactivado.");
            }
        }

        private void SetupNfcReader()
        {
            MainApplication.NfcAdapter.EnableReaderMode(this, new MyNfcReaderCallback(), NfcReaderFlags.NfcA | NfcReaderFlags.SkipNdefCheck, null);
        }

        private class MyNfcReaderCallback : Java.Lang.Object, NfcAdapter.IReaderCallback
        {
            public async void OnTagDiscovered(Tag tag)
            {
                var id = BitConverter.ToString(tag.GetId()).Replace("-", "");
                Console.WriteLine($"Tag ID: {id}");

                // Mostrar el ID en la interfaz gráfica
                MainPage.Instance.MostrarTagID(id);

                // Llamar a la API
                await SendTagIdToApi(id);
            }

            private async Task SendTagIdToApi(string tagId)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        string apiUrl = "https://unphucard.azurewebsites.net/api/ValidarAcceso"; // Cambia por tu URL
                        var jsonContent = JsonConvert.SerializeObject(new { tarjetaId = tagId, aulaSensor = "101" });
                        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Acceso validado correctamente.");
                        }
                        else
                        {
                            Console.WriteLine("Error al validar el acceso: " + response.ReasonPhrase);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al enviar el ID de la tarjeta a la API: " + ex.Message);
                }
            }
        }
    }
}
