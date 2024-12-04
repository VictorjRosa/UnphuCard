using System.Text;
using Newtonsoft.Json;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace UnphuCard_QR
{
    public partial class MainPage : ContentPage
    {
        private bool isScannerActive = false; // Controla si el escáner está activo

        public MainPage()
        {
            InitializeComponent();
        }

        private void ToggleScanner_Clicked(object sender, EventArgs e)
        {
            isScannerActive = !isScannerActive;
            cameraView.IsDetecting = isScannerActive; // Activa o desactiva el escaneo

            var button = sender as Button;
            if (isScannerActive)
            {
                button.Text = "Desactivar Escáner";
                statusLabel2.Text = "Estado: Escáner activado. Listo para escanear.";
            }
            else
            {
                button.Text = "Activar Escáner";
                statusLabel2.Text = "Estado: Escáner desactivado.";
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

                    await EnviarCodigoQR(scannedCode, 1);

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

        private async Task EnviarCodigoQR(string userCode, int estId)
        {
            if (string.IsNullOrEmpty(userCode))
            {
                await DisplayAlert("Error", "El código escaneado está vacío. Intente nuevamente.", "OK");
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

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Éxito", "El código fue enviado correctamente.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", $"No se pudo procesar la solicitud. Error: {response.ReasonPhrase}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al enviar el código: {ex.Message}", "OK");
            }
        }
    }
}
