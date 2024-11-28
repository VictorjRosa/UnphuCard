using Android.App;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using System.Text;
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
                MainPage.Instance?.UpdateStatusLabel("NFC no disponible o desactivado.");
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
                var tagId = BitConverter.ToString(tag.GetId()).Replace("-", "");
                Console.WriteLine($"Tag ID: {tagId}");

                if (MainPage.Instance != null)
                {
                    if (MainPage.Instance.IsDiagnosticMode)
                    {
                        // Solo muestra el Tag ID
                        MainPage.Instance.UpdateStatusLabel($"Tag ID Detectado: {tagId}");
                    }
                    else
                    {
                        // Realiza la validación de acceso
                        var isValid = await SendTagIdToApi(tagId);
                        if (isValid)
                        {
                            MainPage.Instance.ShowAlert("Acceso Permitido", "La tarjeta es válida.");
                        }
                        else
                        {
                            MainPage.Instance.ShowAlert("Acceso Denegado", "La tarjeta no es válida.");
                        }
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
                        return response.IsSuccessStatusCode;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar el ID de la tarjeta a la API: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
