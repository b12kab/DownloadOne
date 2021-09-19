using DownloadOne.View;
using Xamarin.Forms;

namespace DownloadOne
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new DriverView();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
