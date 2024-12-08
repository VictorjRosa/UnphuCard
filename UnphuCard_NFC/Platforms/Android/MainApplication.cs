using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Runtime;

namespace UnphuCard_NFC
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public static NfcAdapter NfcAdapter { get; private set; }
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
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
            NfcManager nfcManager = (NfcManager)ApplicationContext.GetSystemService(Context.NfcService);
            NfcAdapter = nfcManager.DefaultAdapter;

            if (NfcAdapter == null || !NfcAdapter.IsEnabled)
            {
                // NFC no está disponible o está deshabilitado, podrías mostrar una notificación o mensaje aquí si lo deseas
            }
            else
            {
                // NFC está disponible y habilitado, listo para usarse en la app
            }
        }
    }
}