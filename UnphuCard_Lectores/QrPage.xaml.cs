namespace UnphuCard_Lectores
{
    public partial class QrPage : ContentPage
    {
        private bool isScannerActive = false;

        public QrPage()
        {
            InitializeComponent();
        }

        private void ToggleScanner_Clicked(object sender, EventArgs e)
        {
            isScannerActive = !isScannerActive;

            var button = sender as Button;
            if (isScannerActive)
            {
                button.Text = "Desactivar Escáner";
                statusLabelQr.Text = "Escáner activado. Listo para escanear.";
            }
            else
            {
                button.Text = "Activar Escáner";
                statusLabelQr.Text = "Escáner desactivado.";
            }
        }

        private async void CameraView_BarcodeDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        {
            if (!isScannerActive) return;

            isScannerActive = false; // Desactiva el escáner para evitar múltiples lecturas
            try
            {
                var barcodeResult = e.Results.FirstOrDefault();
                if (barcodeResult != null)
                {
                    string scannedCode = barcodeResult.Value;
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        DisplayAlert("Código QR Detectado", scannedCode, "OK"));
                }
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    DisplayAlert("Error al procesar el código", ex.Message, "OK"));
            }
            finally
            {
                await Task.Delay(3000); // Espera 3 segundos antes de reactivar el escáner
                isScannerActive = true;
                statusLabelQr.Text = "Listo para escanear.";
            }
        }
    }
}
