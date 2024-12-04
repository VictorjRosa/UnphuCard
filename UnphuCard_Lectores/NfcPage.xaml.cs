using Android.Nfc;

namespace UnphuCard_Lectores
{
    public partial class NfcPage : ContentPage
    {
        public static NfcPage Instance { get; private set; }

        public NfcPage()
        {
            InitializeComponent();
            Instance = this;
        }

        public void UpdateStatus(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                statusLabelNfc.Text = $"Estado NFC: {message}";
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Configura el lector NFC
            //if (MainApplication.NfcAdapter != null && MainApplication.NfcAdapter.IsEnabled)
            //{
            //    MainApplication.NfcAdapter.EnableReaderMode(
            //        MainActivity.Instance,
            //        new NfcReaderCallback(),
            //        NfcReaderFlags.NfcA | NfcReaderFlags.SkipNdefCheck,
            //        null
            //    );
            //    UpdateStatus("Esperando tarjeta NFC...");
            //}
            //else
            //{
            //    UpdateStatus("NFC no disponible o desactivado.");
            //}
        }

        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    MainApplication.NfcAdapter?.DisableReaderMode(MainActivity.Instance);
        //}

        private class NfcReaderCallback : Java.Lang.Object, NfcAdapter.IReaderCallback
        {
            public async void OnTagDiscovered(Tag tag)
            {
                var tagId = BitConverter.ToString(tag.GetId()).Replace("-", "");

                if (Instance != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Instance.UpdateStatus($"Tag Detectado: {tagId}");
                    });
                }
            }
        }
    }
}
