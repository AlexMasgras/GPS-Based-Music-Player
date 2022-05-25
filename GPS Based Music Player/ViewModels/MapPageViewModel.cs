using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace GPSBasedMusicPlayer
{
    public class MapPageViewModel : ContentView
    {
        public Xamarin.Forms.Maps.Map map;
        private App model;
        private string buttonMode;
        private List<Position> addPointsList;
        private Polyline tempLine;

        public MapPageViewModel(App model, Position pos)
        {
            MapSpan mapSpan = new MapSpan(pos, 0.01, 0.01);
            buttonMode = "newZone";
            ButtonText = "ADD ZONE";
            addPointsList = new List<Position>();


            map = new Xamarin.Forms.Maps.Map(mapSpan)
            {
                MapType = MapType.Hybrid,
                IsShowingUser = true
            };
            map.MapClicked += OnMapClicked;

            tempLine = new Polyline();
            tempLine.StrokeWidth = 8;
            tempLine.StrokeColor = Color.FromHex("#FFAA00");
            map.MapElements.Add(tempLine);

            ZoneAddCommand = new Command(() =>
            {
                ButtonPressed();
            });

            foreach (KeyValuePair<GeoZone, List<Playlist>> entry in model.zoneList)
            {
                if (entry.Key.type.Equals("Circle"))
                {
                    drawCircle(entry.Key.coords);
                }
                else if (entry.Key.type.Equals("Polygon"))
                {
                    drawPolygon(entry.Key.coords);
                }
            }
            this.model = model;
        }

        public Command ZoneAddCommand { get; }

        public string ButtonText { get; set; }

        public async void ButtonPressed()
        {
            if (buttonMode.Equals("newZone"))
            {
                string action = await Application.Current.MainPage.DisplayActionSheet("Type of zone?", "Cancel", null, "Polygon", "Circle");
                if (action.Equals("Circle"))
                {
                    await Application.Current.MainPage.DisplayAlert("Circle", "Tap center of circle, then point on outer edge", "OK");
                    buttonMode = "Circle";
                    ButtonText = "FINISH CIRCLE";
                }
                else if (action.Equals("Polygon"))
                {
                    await Application.Current.MainPage.DisplayAlert("Polygon", "Tap corners of polygon", "OK");
                    buttonMode = "Polygon";
                    ButtonText = "FINISH POLYGON";
                }
            }
            else if (buttonMode.Equals("Circle"))
            {
                if (addPointsList.Count < 2)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Too few points: cancelled", "OK");
                }
                string result = await Application.Current.MainPage.DisplayPromptAsync("Circle", "Name your zone: ");
                drawCircleAndGeo(addPointsList, result);
                addPointsList.Clear();
                tempLine.Geopath.Clear();
                buttonMode = "newZone";
                ButtonText = "ADD ZONE";
            }
            else if (buttonMode.Equals("Polygon"))
            {
                if (addPointsList.Count < 3)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Too few points: cancelled", "OK");
                }
                string result = await Application.Current.MainPage.DisplayPromptAsync("Polygon", "Name your zone: ");
                drawPolygonAndGeo(addPointsList, result);
                addPointsList.Clear();
                tempLine.Geopath.Clear();
                buttonMode = "newZone";
                ButtonText = "ADD ZONE";
            }
        }

        public void OnMapClicked(object sender, MapClickedEventArgs e)
        {
            if (buttonMode.Equals("newZone"))
            {
                return;
            }
            else if (buttonMode.Equals("Circle"))
            {
                addPointsList.Add(e.Position);
                tempLine.Geopath.Add(e.Position);
                if (addPointsList.Count == 2)
                {
                    //automatically presses button, essentially
                    ButtonPressed();
                }
            }
            else if (buttonMode.Equals("Polygon"))
            {
                addPointsList.Add(e.Position);
                tempLine.Geopath.Add(e.Position);
            }
        }
        public static async Task<Position> updateCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);

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


        public void drawPolygon(List<Position> p)
        {
            Polygon polygon = new Polygon
            {
                StrokeWidth = 8,
                StrokeColor = Color.FromHex("#1BA1E2"),
                FillColor = Color.FromHex("#881BA1E2"),
            };
            foreach (Position pos in p)
            {
                polygon.Geopath.Add(pos);
            }

            map.MapElements.Add(polygon);
        }
        public void drawPolygonAndGeo(List<Position> p, string name)
        {
            drawPolygon(p);

            GeoZone zone = new GeoZone(name, ZoneType.POLYGON, new List<Position>(p));
            model.AddZone(zone);
            model.save();
        }

        public void drawCircle(List<Position> p)
        {
            Circle circle = new Circle
            {
                StrokeWidth = 8,
                StrokeColor = Color.FromHex("#1BA1E2"),
                FillColor = Color.FromHex("#881BA1E2"),
                Center = p.ElementAt(0),
                Radius = Distance.BetweenPositions(p.ElementAt(0), p.ElementAt(1))
            };

            map.MapElements.Add(circle);
        }
        public void drawCircleAndGeo(List<Position> p, string name)
        {
            drawCircle(p);

            GeoZone zone = new GeoZone(name, ZoneType.CIRCLE, new List<Position>(p));
            model.AddZone(zone);
            model.save();
        }
    }
}