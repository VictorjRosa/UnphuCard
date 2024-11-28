using System.Text;
using Newtonsoft.Json;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace UnphuCard_Lectores
{
    public partial class MainPage : ContentPage
    {
        public static MainPage Instance { get; private set; }
        private bool isDiagnosticMode = false; // Bandera para alternar entre los modos

        public MainPage()
        {
            InitializeComponent();
            Instance = this; // Guarda la referencia a esta página
        }

        public void ShowAlert(string title, string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert(title, message, "OK");
            });
        }

        public void UpdateStatusLabel(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                statusLabel.Text = message;
            });
        }

        // Método del botón para alternar entre los modos
        private void ToggleMode_Clicked(object sender, EventArgs e)
        {
            isDiagnosticMode = !isDiagnosticMode;

            var button = sender as Button;
            if (isDiagnosticMode)
            {
                button.Text = "Modo Validación: Validar Acceso";
                UpdateStatusLabel("Modo Diagnóstico activado. Escanee una tarjeta para mostrar su Tag ID.");
            }
            else
            {
                button.Text = "Modo Diagnóstico: Mostrar Tag ID";
                UpdateStatusLabel("Modo Validación activado. Escanee una tarjeta para validar acceso.");
            }
        }

        private async void CameraView_BarcodeDetected(object sender, BarcodeDetectionEventArgs e)
        {
            try
            {
                var barcodeResult = e.Results.FirstOrDefault();
                if (barcodeResult != null)
                {
                    string scannedCode = barcodeResult.Value;

                    // Muestra el código escaneado
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        DisplayAlert("Código Escaneado", scannedCode, "OK"));

                    // Si no estás en modo diagnóstico, envía el código QR a la API
                    if (!isDiagnosticMode)
                    {
                        await EnviarCodigoQR(scannedCode);
                    }
                }
            }
            catch (Exception ex)
            {
                // Muestra el error en un mensaje y registra los detalles en la consola
                await MainThread.InvokeOnMainThreadAsync(() =>
                    DisplayAlert("Error", $"Error al procesar el código: {ex.Message}", "OK"));
                Console.WriteLine($"Error en CameraView_BarcodeDetected: {ex}");
            }
        }

        private async Task EnviarCodigoQR(string userCode)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string apiUrl = "https://unphucard.azurewebsites.net/api/PagarCompra";
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
                        await DisplayAlert("Error", "No se pudo procesar la solicitud. Intente nuevamente.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al enviar el código: {ex.Message}", "OK");
            }
        }

        public bool IsDiagnosticMode => isDiagnosticMode; // Para acceder al modo actual desde MainActivity
    }
}
