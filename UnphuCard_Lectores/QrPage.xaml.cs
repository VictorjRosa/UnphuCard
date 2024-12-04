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
                UpdateStatus("Escáner activado. Listo para escanear.");
            }
            else
            {
                button.Text = "Activar Escáner";
                UpdateStatus("Escáner desactivado.");
            }
        }

        private async void CameraView_BarcodeDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        {
            if (!isScannerActive) return;

            isScannerActive = false; // Pausa el escáner temporalmente
            var barcodeResult = e.Results.FirstOrDefault();
            if (barcodeResult != null)
            {
                await DisplayAlert("Código Detectado", barcodeResult.Value, "OK");
            }
            UpdateStatus("Listo para escanear.");
            isScannerActive = true; // Reactiva el escáner
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
