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

        public void UpdateStatusLabelNfc(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                statusLabel.Text = $"Estado NFC: {message}";
            });
        }

        public void UpdateStatusLabelQr(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                statusLabel2.Text = $"Estado QR: {message}";
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
                UpdateStatusLabelNfc("Modo Diagnóstico activado. Escanee una tarjeta para mostrar su Tag ID.");
            }
            else
            {
                button.Text = "Modo Diagnóstico: Mostrar Tag ID";
                UpdateStatusLabelNfc("Modo Validación activado. Escanee una tarjeta para validar acceso.");
            }
        }

        private bool isScannerActive = false; // Controla si el escáner está activo

        private void ToggleScanner_Clicked(object sender, EventArgs e)
        {
            isScannerActive = !isScannerActive;

            var button = sender as Button;
            if (isScannerActive)
            {
                button.Text = "Desactivar Escáner";
                UpdateStatusLabelQr("Escáner activado. Listo para escanear.");
            }
            else
            {
                button.Text = "Activar Escáner";
                UpdateStatusLabelQr("Escáner desactivado.");
            }
        }

        private async void CameraView_BarcodeDetected(object sender, BarcodeDetectionEventArgs e)
        {
            if (!isScannerActive) return; // Si el escáner está desactivado, no haga nada

            isScannerActive = false; // Desactiva el escáner tras un escaneo exitoso
            try
            {
                var barcodeResult = e.Results.FirstOrDefault();
                if (barcodeResult != null)
                {
                    string scannedCode = barcodeResult.Value;

                    if (isDiagnosticMode)
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                            DisplayAlert("Modo Diagnóstico", $"Tag ID: {scannedCode}", "OK"));
                    }
                    else
                    {
                        await EnviarCodigoQR(scannedCode);
                    }
                }
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    DisplayAlert("Error al procesar el código", $"Detalle del error: {ex.Message}", "OK"));
            }

            finally
            {
                UpdateStatusLabelQr("Escaneo en proceso...");
                await Task.Delay(3000);
                UpdateStatusLabelQr("Listo para escanear.");
                isScannerActive = true; // Reactiva el escaneo
            }
        }

        private async Task EnviarCodigoQR(string userCode, int estId = 1)
        {
            if (string.IsNullOrEmpty(userCode))
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                DisplayAlert("Error", "El código escaneado está vacío. Intente nuevamente.", "OK"));
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
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        DisplayAlert("Éxito", "El código fue enviado correctamente.", "OK"));
                    }
                    else
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        DisplayAlert("Error", $"No se pudo procesar la solicitud. Error: {response.ReasonPhrase}", "OK"));
                    }
                }
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                DisplayAlert("Error", $"Error al enviar el código: {ex.Message}", "OK"));
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }


        public bool IsDiagnosticMode => isDiagnosticMode; // Para acceder al modo actual desde MainActivity
    }
}
