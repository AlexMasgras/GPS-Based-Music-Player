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
        public ObservableCollection<Playlist> masterList { get; }
        public Dictionary<GeoZone, List<Playlist>> zoneList { get; }

        private List<GeoZone> currentZones;

        public GeoZone refZone;
        public MainPageViewModel()
        {
            zoneList = new Dictionary<GeoZone, List<Playlist>>();
            masterList = new ObservableCollection<Playlist>();
            currentZones = new List<GeoZone>();

            
            //deserialize
            if (File.Exists(Path.Combine(FileSystem.AppDataDirectory + "/data.json")))
            {
                string file = Path.Combine(FileSystem.AppDataDirectory + "/data.json");

                string fileContents = File.ReadAllText(file);

                Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContents);

                foreach (string z in data.Keys)
                {
                    GeoZone zone = JsonConvert.DeserializeObject<GeoZone>(z);
                    List<Playlist> lists = new List<Playlist>();
                    foreach(string p in JsonConvert.DeserializeObject<List<string>>(data[z]))
                    {
                        Playlist pl = new Playlist(JsonConvert.DeserializeObject<List<string>>(p));

                        bool containsDupe = false;
                        foreach(Playlist plt in masterList)
                        {
                            if(pl.functionallyEquals(plt))
                            {
                                containsDupe = true;
                                lists.Add(plt);
                                plt.onZoneBind(zone);
                                break;
                            }
                        }

                        if(!containsDupe)
                        {
                            lists.Add(pl);
                            masterList.Add(pl);
                            pl.onZoneBind(zone);
                        }
                    }
                    zoneList.Add(zone, lists);
                    if(zone.type.Equals("REFERENCE"))
                    {
                        refZone = zone;
                    }
                }
            }
            else
            {
                zoneList.Add(refZone = new GeoZone(null, "REFERENCE", new List<Position>()), new List<Playlist>());
            }

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

            CrossMediaManager.Current.Init();
            
            //This is the main loop of the app.
            //Every seven seconds, it checks the user's location, and sees if they're in any zones.
            //If so, it plays the shuffled sum of playlists from all zones the user is in.
            Device.StartTimer(TimeSpan.FromSeconds(4),() =>
            {
                Task.Run(async () =>
                {
                    //save data
                    //save();

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

        public void save()
        {
            string file = Path.Combine(FileSystem.AppDataDirectory + "/data.json");

            Dictionary<string, string> serZones = new Dictionary<string, string>();
            foreach(GeoZone z in zoneList.Keys)
            {
                string zone = JsonConvert.SerializeObject(z);
                List<string> lists = new List<string>();
                foreach(Playlist p in zoneList[z])
                {
                    string s = p.serialize();
                    lists.Add(s);
                }
                serZones.Add(zone, JsonConvert.SerializeObject(lists));
            }

            string jsonfile = JsonConvert.SerializeObject(serZones, Formatting.Indented);

            File.WriteAllText(file, jsonfile);
        }
    }
}