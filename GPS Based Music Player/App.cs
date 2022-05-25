using MediaManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace GPSBasedMusicPlayer
{
    public class App : Xamarin.Forms.Application
    {
        public ObservableCollection<Playlist> masterList { get; }
        public Dictionary<GeoZone, List<Playlist>> zoneList { get; }

        private List<GeoZone> currentZones;

        private GeoZone refZone;
        public App()
        {
            //Load key App data structures
            zoneList = new Dictionary<GeoZone, List<Playlist>>();
            masterList = new ObservableCollection<Playlist>();
            currentZones = new List<GeoZone>();

            //Deserialize saved data
            if (File.Exists(Path.Combine(FileSystem.AppDataDirectory + "/data.json")))
            {
                string file = Path.Combine(FileSystem.AppDataDirectory + "/data.json");

                string fileContents = File.ReadAllText(file);

                Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContents);

                foreach (string z in data.Keys)
                {
                    //Create saved GeoZone
                    GeoZone zone = JsonConvert.DeserializeObject<GeoZone>(z);
                    List<Playlist> lists = new List<Playlist>();

                    //Add playlists to GeoZone
                    foreach (string p in JsonConvert.DeserializeObject<List<string>>(data[z]))
                    {
                        Playlist pl = new Playlist(JsonConvert.DeserializeObject<List<string>>(p));

                        bool containsDupe = false;
                        //Check if each song is a duplicate
                        //Add true song to playlist
                        foreach (Playlist plt in masterList)
                        {
                            if (pl.functionallyEquals(plt))
                            {
                                containsDupe = true;
                                lists.Add(plt);
                                plt.onZoneBind(zone);
                                break;
                            }
                        }

                        if (!containsDupe)
                        {
                            lists.Add(pl);
                            masterList.Add(pl);
                            pl.onZoneBind(zone);
                        }
                    }
                    zoneList.Add(zone, lists);

                    //If it finds the reference zone, assign to refZone variable
                    if (zone.type == ZoneType.REFERENCE)
                    {
                        refZone = zone;
                    }
                }
            }
            else
            {
                //If there was no data to deserialize, the reference zone must be created
                zoneList.Add(refZone = new GeoZone(null, ZoneType.REFERENCE, new List<Position>()), new List<Playlist>());
            }

            //Initialize the media manager
            CrossMediaManager.Current.Init();

            //Create MainPage for user to navigate through app
            var navigationPage = new Xamarin.Forms.NavigationPage(new MainPage(this))
            {
                BarBackgroundColor = Color.FromHex("0099B4"),
                BarTextColor = Color.White
            };
            

            navigationPage.On<iOS>().SetPrefersLargeTitles(true);

            MainPage = navigationPage;

            //This is the main loop of the app.
            //Every seven seconds, it checks the user's location, and sees if they're in any zones.
            //If so, it plays the shuffled sum of playlists from all zones the user is in.
            Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                Task.Run(async () =>
                {
                    //update current position
                    Position currentPos = await updateCurrentLocation();
                    List<GeoZone> newZones = new List<GeoZone>();

                    //Collision detection: rework to be faster if possible in time
                    foreach (GeoZone z in zoneList.Keys)
                    {
                        if (z.type.Equals("Circle") && GeoZone.circlePoint(z.coords, currentPos))
                        {
                            newZones.Add(z);
                        }
                        else if (z.type.Equals("Polygon") && GeoZone.polyPoint(z.coords, currentPos.Longitude, currentPos.Latitude))
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
                    foreach (GeoZone z in currentZones)
                    {
                        foreach (Playlist p in zoneList[z])
                        {
                            if (!combinedList.Contains(p))
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

        public void addToRef(Playlist list)
        {
            zoneList[refZone].Add(list);
        }

        public List<GeoZone> getCurrentZones()
        {
            return new List<GeoZone>(currentZones);
        }

        protected override void OnSleep()
        {
            save();
            base.OnSleep();
        }

        public void save()
        {
            string file = Path.Combine(FileSystem.AppDataDirectory + "/data.json");

            Dictionary<string, string> serZones = new Dictionary<string, string>();
            foreach (GeoZone z in zoneList.Keys)
            {
                string zone = JsonConvert.SerializeObject(z);
                List<string> lists = new List<string>();
                foreach (Playlist p in zoneList[z])
                {
                    string s = p.serialize();
                    lists.Add(s);
                }
                serZones.Add(zone, JsonConvert.SerializeObject(lists));
            }

            string jsonfile = JsonConvert.SerializeObject(serZones, Formatting.Indented);

            File.WriteAllText(file, jsonfile);
        }

        public static async Task<Position> updateCurrentLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    return new Position(location.Latitude, location.Longitude);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine(fnsEx);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Console.WriteLine(fneEx);
            }
            catch (PermissionException pEx)
            {
                Console.WriteLine(pEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new Position(0, 0);
        }
    }
}
