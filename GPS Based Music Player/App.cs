using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace GPSBasedMusicPlayer
{
    public class App : Xamarin.Forms.Application
    {
        public App()
        {
            var navigationPage = new Xamarin.Forms.NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.FromHex("0099B4"),
                BarTextColor = Color.White
            };

            navigationPage.On<iOS>().SetPrefersLargeTitles(true);

            MainPage = navigationPage;
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }
    }
}
