using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using MediaManager;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using System.IO;
using Newtonsoft.Json;

namespace GPSBasedMusicPlayer
{
    public class MainPageViewModel : ContentView
    {
        private App app;
        public MainPageViewModel(App context)
        {
            MapMenuCommand = new Command(async () =>
            {
                Position currentPos = await App.updateCurrentLocation();
                MapPageViewModel model = new MapPageViewModel(app, currentPos);
                MapPage page = new MapPage(model);
                page.BindingContext = model;
                
                await Application.Current.MainPage.Navigation.PushAsync(page);
            });

            MusicMenuCommand = new Command(async () =>
            {
                MusicPageViewModel model = new MusicPageViewModel(app);
                MusicPage page = new MusicPage(model);
                page.BindingContext = model;

                await Application.Current.MainPage.Navigation.PushAsync(page);
            });

            app = context;
        }
        public Command MapMenuCommand { get; }

        public string CurrentSong { get => CrossMediaManager.Current.Queue.Current.Artist + " - " + CrossMediaManager.Current.Queue.Current.Title; }

        public string CurrentZone
        {
            get => getCurrentZones();
        }

        private string getCurrentZones()
        {
            if(app.getCurrentZones().Count == 0)
            {
                return "None";
            }
            StringBuilder sb = new StringBuilder();
            List<GeoZone> gzs = app.getCurrentZones();
            for(int i = 0; i < gzs.Count; i++)
            {
                sb.Append(gzs[i].name);
                if(i != gzs.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            return sb.ToString();
        }

        public Command MusicMenuCommand { get; }
    }
}