namespace UnphuCard_Lectores
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Configura el menú como página principal con navegación
            MainPage = new NavigationPage(new MenuPage());
        }
    }
}
