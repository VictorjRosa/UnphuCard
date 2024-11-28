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
                statusLabel2.Text = message;
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

        private bool isScanning = true; // Controla si el lector está activo

        private async void CameraView_BarcodeDetected(object sender, BarcodeDetectionEventArgs e)
        {
            if (!isScanning) return; // Si ya se está procesando un escaneo, no haga nada

            isScanning = false; // Desactiva el escaneo hasta que se complete el proceso
            try
            {
                var barcodeResult = e.Results.FirstOrDefault();
                if (barcodeResult != null)
                {
                    string scannedCode = barcodeResult.Value;

                    if (isDiagnosticMode)
                    {
                        // Mostrar el Tag ID en modo diagnóstico
                        await MainThread.InvokeOnMainThreadAsync(() =>
                            DisplayAlert("Modo Diagnóstico", $"Tag ID: {scannedCode}", "OK"));
                    }
                    else
                    {
                        // Enviar el código QR a la API
                        await EnviarCodigoQR(scannedCode);
                    }
                }
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    DisplayAlert("Error", $"Error al procesar el código: {ex.Message}", "OK"));
            }
            finally
            {
                UpdateStatusLabel("Escaneo en proceso...");
                await Task.Delay(3000);
                UpdateStatusLabel("Listo para escanear.");
                isScanning = true; // Reactiva el escaneo
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
