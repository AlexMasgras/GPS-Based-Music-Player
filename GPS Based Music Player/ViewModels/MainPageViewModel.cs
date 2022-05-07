using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using MediaManager;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GPSBasedMusicPlayer
{
    public class MainPageViewModel : ContentView
    {
        public ObservableCollection<Playlist> masterList { get; }
        public Dictionary<GeoZone, List<Playlist>> zoneList { get; }

        private List<GeoZone> currentZones;
        public MainPageViewModel()
        {
            zoneList = new Dictionary<GeoZone, List<Playlist>>();
            currentZones = new List<GeoZone>();

            MapMenuCommand = new Command(async () =>
            {
                Position currentPos = await MapPageViewModel.updateCurrentLocation();
                MapPage page = new MapPage(this, currentPos);
                
                await Application.Current.MainPage.Navigation.PushAsync(page);
            });

            MusicMenuCommand = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new MusicPage(this));
            });

            masterList = new ObservableCollection<Playlist>();
            CrossMediaManager.Current.Init();

            //This is the main loop of the app.
            //Every seven seconds, it checks the user's location, and sees if they're in any zones.
            //If so, it plays the shuffled sum of playlists from all zones the user is in.
            Device.StartTimer(TimeSpan.FromSeconds(4),() =>
            {
                Task.Run(async () =>
                {
                    //update current position
                    Position currentPos = await MapPageViewModel.updateCurrentLocation();
                    List<GeoZone> newZones = new List<GeoZone>();

                    //Collision detection: rework to be faster if possible in time
                    foreach (GeoZone z in zoneList.Keys)
                    {
                        if (z.type.Equals("Circle") && GeoZone.circlePoint(z.coords, currentPos))
                        {
                            newZones.Add(z);
                        }
                        else if(z.type.Equals("Polygon") && GeoZone.polyPoint(z.coords, currentPos.Longitude, currentPos.Latitude))
                        {
                            newZones.Add(z);
                        }
                    }

                    //if still in same zone/zones, don't reshuffle/reassign list
                    if (newZones.Count == currentZones.Count)
                    {
                        for (int i = 0; i < newZones.Count; i++)
                        {
                            //if any songs are different, break, reset playback
                            if (newZones[i] != currentZones[i])
                            {
                                break;
                            }

                            //if we're on the last zone, return: all are the same
                            if (i == newZones.Count - 1)
                            {
                                return;
                            }
                        }
                    }

                    //if not in same zone, update zone list
                    currentZones = newZones;
                    newZones = null;

                    //combine playlists in multiple zones
                    List<Playlist> combinedList = new List<Playlist>();
                    foreach(GeoZone z in currentZones)
                    {
                        foreach(Playlist p in zoneList[z])
                        {
                            if(!combinedList.Contains(p))
                            {
                                combinedList.Add(p);
                            }
                        }
                    }

                    //play combined playlist
                    SongPlayer.Play(combinedList);

                });
                return true;
            });
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
    }
}