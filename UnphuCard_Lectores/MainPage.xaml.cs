namespace UnphuCard_Lectores
{
    public partial class MainPage : ContentPage
    {
        public static MainPage Instance { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            Instance = this; // Asignar la instancia actual
        }

        public void MostrarTagID(string tagId)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DisplayAlert("ID de Tarjeta Detectada", $"ID: {tagId}", "OK");
            });
        }
    }
}
