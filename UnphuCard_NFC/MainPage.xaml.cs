namespace UnphuCard_NFC
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
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    statusLabel.Text = message;
                });
            });
        }

        // Método del botón para alternar entre los modos
        private void ToggleMode_Clicked(object sender, EventArgs e)
        {
            isDiagnosticMode = !isDiagnosticMode;

            var button = sender as Button;
            if (isDiagnosticMode)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    button.Text = "Modo Validación: Validar Acceso";
                    UpdateStatusLabel("Modo Diagnóstico activado. Escanee una tarjeta para mostrar su Tag ID.");
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    button.Text = "Modo Diagnóstico: Mostrar Tag ID";
                    UpdateStatusLabel("Modo Validación activado. Escanee una tarjeta para validar acceso.");
                });
            }
        }

        public bool IsDiagnosticMode => isDiagnosticMode; // Para acceder al modo actual desde MainActivity
    }
}
