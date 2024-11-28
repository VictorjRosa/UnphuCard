using Newtonsoft.Json;
using ZXing.Net.Maui.Controls;
using System.Text;

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

        public bool IsDiagnosticMode => isDiagnosticMode; // Para acceder al modo actual desde MainActivity

        private async void OnScanQrClicked(object sender, EventArgs e)
        {
            // Abrir la cámara para escanear QR
            var scanner = new CameraBarcodeReaderView();
            var result = await scanner.ReadAsync();

            if (result != null)
            {
                string userCode = result.Value; // Extrae el UsuCodigo desde el QR
                ResultLabel.Text = $"Código escaneado: {userCode}";

                // Envía el código escaneado a la API para procesar la compra
                await EnviarCodigoQR(userCode);
            }
            else
            {
                ResultLabel.Text = "No se pudo escanear el QR. Intente nuevamente.";
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
    }
}
