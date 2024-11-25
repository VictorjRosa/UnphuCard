namespace UnphuCard_Lectores
{
    public partial class MainPage : ContentPage
    {
        public static MainPage Instance { get; private set; }

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
    }
}
