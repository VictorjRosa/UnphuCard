using System.Text;
using Newtonsoft.Json;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace UnphuCard_QR
{
    public partial class MainPage : ContentPage
    {
        private bool isScanning = true; // Controla si el escáner está activo

        public MainPage()
        {
            InitializeComponent();
        }

        private async void CameraView_BarcodeDetected(object sender, BarcodeDetectionEventArgs e)
        {
            if (!isScanning) return; // Evitar múltiples escaneos

            isScanning = false; // Desactiva temporalmente el escáner
            try
            {
                var barcodeResult = e.Results.FirstOrDefault();
                if (barcodeResult != null)
                {
                    string scannedCode = barcodeResult.Value;
                    await EnviarCodigoQR(scannedCode);
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo escanear el código. Intente nuevamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al procesar el código: {ex.Message}", "OK");
            }
            finally
            {
                await Task.Delay(3000); // Agregar un retraso antes de reactivar
                isScanning = true;
            }
        }

        private async Task EnviarCodigoQR(string userCode)
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
                    string apiUrl = "https://unphucard.azurewebsites.net/api/PagarCompra";
                    var payload = new { UsuCodigo = userCode };
                    var jsonContent = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Éxito", "Código QR procesado correctamente.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", $"No se pudo procesar la solicitud: {response.ReasonPhrase}", "OK");
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
