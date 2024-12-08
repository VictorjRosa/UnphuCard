using System.Text;
using Android.OS;
using Newtonsoft.Json;
using UnphuCard_QR.Services;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using Android.Provider;

namespace UnphuCard_QR
{
    public partial class MainPage : ContentPage
    {
        private bool isScannerActive = false; // Controla si el escáner está activo

        public MainPage()
        {
            InitializeComponent();
            InicializarEstablecimientoId();
        }

        private void ToggleScanner_Clicked(object sender, EventArgs e)
        {
            isScannerActive = !isScannerActive;
            cameraView.IsDetecting = isScannerActive; // Activa o desactiva el escaneo

            var button = sender as Button;
            if (isScannerActive)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    button.Text = "Desactivar Escáner";
                    statusLabel2.Text = "Estado: Escáner activado. Listo para escanear.";
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    button.Text = "Activar Escáner";
                    statusLabel2.Text = "Estado: Escáner desactivado.";
                });
            }
        }

        private int? estId; // EstId

        private async void InicializarEstablecimientoId()
        {
            var androidId = Android.Provider.Settings.Secure.GetString(
                Android.App.Application.Context.ContentResolver,
                Android.Provider.Settings.Secure.AndroidId
            );

            var apiService = new ApiService();
            estId = await apiService.ObtenerEstablecimientoId(androidId);

            if (estId == null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "No se pudo obtener el EstId para este dispositivo.", "OK");
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Éxito", $"EstablecimientoId obtenido: {estId}", "OK");
                });
            }
        }

        private async void CameraView_BarcodeDetected(object sender, BarcodeDetectionEventArgs e)
        {
            if (!isScannerActive) return; // Si el escáner está desactivado, no haga nada

            isScannerActive = false; // Desactiva el escáner tras un escaneo exitoso
            cameraView.IsDetecting = false; // Detiene el escaneo hasta que el usuario lo reactive

            try
            {
                var barcodeResult = e.Results.FirstOrDefault();
                if (barcodeResult != null)
                {
                    string scannedCode = barcodeResult.Value;

                    await EnviarCodigoQR(scannedCode);

                    // Actualiza la UI desde el hilo principal
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        statusLabel2.Text = "Código procesado correctamente.";
                    });
                }
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error al procesar el código", $"Detalle del error: {ex.Message}", "OK");
                });
            }
            finally
            {
                // Reactiva el escáner después de un tiempo
                await Task.Delay(3000);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    statusLabel2.Text = "Listo para escanear.";
                    isScannerActive = true;
                    cameraView.IsDetecting = true;
                });
            }
        }

        private async Task EnviarCodigoQR(string userCode)
        {
            if (string.IsNullOrEmpty(userCode))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "El código escaneado está vacío. Intente nuevamente.", "OK");
                });
                return;
            }

            if (estId == null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "El EstId no está configurado. Intente nuevamente.", "OK");
                });
                return;
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    string apiUrl = $"https://unphucard.azurewebsites.net/api/EditarSesion/{estId}";
                    var payload = new
                    {
                        UsuCodigo = userCode
                    };

                    var jsonContent = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await httpClient.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert("Éxito", "El código fue enviado correctamente.", "OK");
                        });
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert("Error", $"No se pudo procesar la solicitud. Error: {response.ReasonPhrase}", "OK");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", $"Error al enviar el código: {ex.Message}", "OK");
                });
            }
        }
    }
}
