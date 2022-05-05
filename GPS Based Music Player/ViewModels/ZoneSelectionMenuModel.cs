using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace GPSBasedMusicPlayer
{
    public class ZoneSelectionMenuModel : ContentView
    {
        GeoZone chosenZone;
        public ZoneSelectionMenuModel()
        {
            chosenZone = null;

            returnItem = new Command( () =>
            {
                MessagingCenter.Send(this, "a", chosenZone);
                Application.Current.MainPage.Navigation.PopModalAsync(true);
            });
        }

        public Command returnItem { get; }
    }
}