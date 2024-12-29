using Android.App;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;
using Newtonsoft.Json;
using System.Text;
using UnphuCard_NFC.Services;

namespace UnphuCard_NFC
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public MainActivity()
        {
            InicializarEstablecimientoId();
        }

        private int? dispId; // DispId

        private async void InicializarEstablecimientoId()
        {
            var androidId = Android.Provider.Settings.Secure.GetString(
                Android.App.Application.Context.ContentResolver,
                Android.Provider.Settings.Secure.AndroidId
            );

            var apiService = new ApiService();
            dispId = await apiService.ObtenerEstablecimientoId(androidId);

            if (dispId == null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage.Instance.ShowAlert("Error", "No se pudo obtener el DispId para este dispositivo.");
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage.Instance.ShowAlert("Éxito", $"DispositivoId obtenido: {dispId}");
                });
            }
        }

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
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage.Instance?.UpdateStatusLabel("NFC no disponible o desactivado.");
                });
            }
        }

        private void SetupNfcReader()
        {
            if (dispId.HasValue) // Verifica que dispId tiene un valor antes de pasarla al callback
            {
                MainApplication.NfcAdapter.EnableReaderMode(this, new MyNfcReaderCallback(dispId.Value), NfcReaderFlags.NfcA | NfcReaderFlags.SkipNdefCheck, null);
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage.Instance?.UpdateStatusLabel("DispId no disponible.");
                });
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            // Reinicia el lector NFC si la actividad vuelve al primer plano
            if (dispId.HasValue && MainApplication.NfcAdapter != null && MainApplication.NfcAdapter.IsEnabled)
            {
                SetupNfcReader();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            // Desactiva el lector NFC cuando la actividad pase a segundo plano
            if (MainApplication.NfcAdapter != null)
            {
                MainApplication.NfcAdapter.DisableReaderMode(this);
            }
        }

        private class MyNfcReaderCallback : Java.Lang.Object, NfcAdapter.IReaderCallback
        {
            private int dispId;
            public MyNfcReaderCallback(int dispId)
            {
                this.dispId = dispId;
            }
            public async void OnTagDiscovered(Tag tag)
            {
                var tagId = BitConverter.ToString(tag.GetId()).Replace("-", "");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Console.WriteLine($"Tag ID: {tagId}");
                });

                if (MainPage.Instance != null)
                {
                    if (MainPage.Instance.IsDiagnosticMode)
                    {
                        // Solo muestra el Tag ID
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            MainPage.Instance.UpdateStatusLabel($"Tag ID Detectado: {tagId}");
                        });
                    }
                    else
                    {
                        // Realiza la validación de acceso
                        var isValid = await SendTagIdToApi(tagId);
                        if (isValid)
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                MainPage.Instance.ShowAlert("Acceso Permitido", "La tarjeta es válida.");
                            });
                        }
                        else
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                MainPage.Instance.ShowAlert("Acceso Denegado", "La tarjeta no es válida.");
                            });
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
                        var jsonContent = JsonConvert.SerializeObject(new { TarjCodigo = tagId, AulaSensor = $"{dispId}" });
                        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync(apiUrl, content);
                        return response.IsSuccessStatusCode;
                    }
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Console.WriteLine($"Error al enviar el ID de la tarjeta a la API: {ex.Message}");
                    });
                    return false;
                }
            }
        }
    }
}
