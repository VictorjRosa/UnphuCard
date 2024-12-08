using Newtonsoft.Json;
using System.Text;

namespace UnphuCard_Lectores
{
    public partial class NfcPage : ContentPage
    {
        private bool isDiagnosticMode = false; // Bandera para alternar entre modos

        public NfcPage()
        {
            InitializeComponent();
        }

        public void OnNfcTagScanned(string tagId)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (isDiagnosticMode)
                {
                    // Muestra solo el Tag ID en modo diagnóstico
                    await DisplayAlert("Modo Diagnóstico", $"Tag ID: {tagId}", "OK");
                    UpdateStatus($"Tag ID Detectado: {tagId}");
                }
                else
                {
                    // Valida el acceso
                    UpdateStatus("Validando acceso...");
                    await ValidarAcceso(tagId);
                }
            });
        }

        private async Task ValidarAcceso(string tagId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string apiUrl = "https://unphucard.azurewebsites.net/api/ValidarAcceso";
                    var payload = new { TarjCodigo = tagId, AulaSensor = "101" };
                    var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Acceso Permitido", "La tarjeta es válida.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Acceso Denegado", "La tarjeta no es válida.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al validar el acceso: {ex.Message}", "OK");
            }
            finally
            {
                UpdateStatus("Esperando escaneo...");
            }
        }

        private void ToggleMode_Clicked(object sender, EventArgs e)
        {
            isDiagnosticMode = !isDiagnosticMode;

            var button = sender as Button;
            if (isDiagnosticMode)
            {
                button.Text = "Modo Validación";
                UpdateStatus("Modo Diagnóstico activado. Escanee una tarjeta para mostrar su Tag ID.");
            }
            else
            {
                button.Text = "Modo Diagnóstico";
                UpdateStatus("Modo Validación activado. Escanee una tarjeta para validar acceso.");
            }
        }

        private void UpdateStatus(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                statusLabel.Text = message;
            });
        }
    }
}
