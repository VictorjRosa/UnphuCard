using Android.App;
using Android.Nfc;
using Android.Runtime;

namespace UnphuCard_Lectores
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public static NfcAdapter NfcAdapter { get; private set; }

        public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override void OnCreate()
        {
            base.OnCreate();
            InitializeNfc();
        }

        private void InitializeNfc()
        {
            NfcManager nfcManager = (NfcManager)ApplicationContext.GetSystemService(Android.Content.Context.NfcService);
            NfcAdapter = nfcManager.DefaultAdapter;

            if (NfcAdapter == null || !NfcAdapter.IsEnabled)
            {
                // NFC no está disponible, agrega logs o acciones necesarias
            }
        }
    }
}
