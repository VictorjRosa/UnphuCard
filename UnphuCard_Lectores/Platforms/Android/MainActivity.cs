using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Android.OS;

namespace UnphuCard_Lectores
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (MainApplication.NfcAdapter != null && MainApplication.NfcAdapter.IsEnabled)
            {
                SetupNfcForegroundDispatch();
            }
            else
            {
                Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", "NFC no está habilitado en este dispositivo.", "OK");
            }
        }

        private void SetupNfcForegroundDispatch()
        {
            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable); // Se agregó FLAG_IMMUTABLE
            var filters = new[] { new IntentFilter(NfcAdapter.ActionTagDiscovered) };

            MainApplication.NfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (NfcAdapter.ActionTagDiscovered.Equals(intent.Action))
            {
                var tag = (Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
                var tagId = BitConverter.ToString(tag.GetId()).Replace("-", "");

                // Redirige el evento a NfcPage si está activa
                if (Microsoft.Maui.Controls.Application.Current.MainPage is NavigationPage navigationPage &&
                    navigationPage.CurrentPage is NfcPage nfcPage)
                {
                    nfcPage.OnNfcTagScanned(tagId);
                }
            }
        }
    }
}
