namespace UnphuCard_Lectores
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private async void OnNfcPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NfcPage());
        }

        private async void OnQrPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new QrPage());
        }
    }
}
