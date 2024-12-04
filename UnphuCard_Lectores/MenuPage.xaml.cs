namespace UnphuCard_Lectores
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        // Método para navegar a la página de NFC
        private async void OnNfcButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NfcPage());
        }

        // Método para navegar a la página de QR
        private async void OnQrButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new QrPage());
        }
    }
}
