using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MediaManager;

using Xamarin.Forms;

namespace GPSBasedMusicPlayer
{
    public class MainPageViewModel : ContentView
    {
        public ObservableCollection<Playlist> masterList { get; }
        public Dictionary<GeoZone, List<Playlist>> zoneList { get; }
        public MainPageViewModel()
        {
            zoneList = new Dictionary<GeoZone, List<Playlist>>();

            MapMenuCommand = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new MapPage(this));
            });

            MusicMenuCommand = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new MusicPage(this));
            });

            masterList = new ObservableCollection<Playlist>();
            CrossMediaManager.Current.Init();
        }

        public void AddZone(GeoZone zone)
        {
            zoneList.Add(zone, new List<Playlist>());
        }

        public void RemoveZone(GeoZone zone)
        {
            zoneList.Remove(zone);
        }

        public Command MapMenuCommand { get; }

        public Command MusicMenuCommand { get; }

        public Song CurrentSong { get; }
    }
}