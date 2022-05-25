using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace GPSBasedMusicPlayer
{
    public class MusicPageViewModel : ContentPage
    {
        private App app;
        public MusicPageViewModel(App model)
        {
            AddNew = new Command( async () =>
            {
                string result = await Application.Current.MainPage.DisplayPromptAsync("Playlist Creation", "Name your Playlist");
                Playlist p = new Playlist(result);
                model.masterList.Add(p);
                model.addToRef(p);
                model.save();
            });

            app = model;
        }

        public Command AddNew { get; }

        public IList<Playlist> getMaster { get => app.masterList; }

        public Command ButtonPressed { get; }

        public async void OnTap(object sender, ItemTappedEventArgs e)
        {
            string action = await DisplayActionSheet("Playlist: " + e.Item.ToString(), "Cancel", "Delete", "Edit", "Rename", "Play", "Assign to Zone", "Unassign from Zone");
            GeoZone z = null;
            if (action.Equals("Assign to Zone") || action.Equals("Unassign from Zone"))
            {
                MessagingCenter.Subscribe<ZoneSelectionMenu, GeoZone>(this, "a", (send, arg) =>
                {
                    PlaylistMenu((Playlist)e.Item, app, action, arg);
                    MessagingCenter.Unsubscribe<ZoneSelectionMenu, GeoZone>(this, "a");
                });

                bool add = action.Equals("Assign to Zone");
                List<GeoZone> list = add ? new List<GeoZone>(app.zoneList.Keys.ToList()) : ((Playlist)e.Item).getBoundZones();

                if (add)
                {
                    List<GeoZone> toDisplay = new List<GeoZone>();
                    foreach (GeoZone zone in list)
                    {
                        if (zone.ToString() != null && !((Playlist)e.Item).getBoundZones().Contains(zone))
                        {
                            toDisplay.Add(zone);
                        }
                    }
                    list = toDisplay;
                }

                ZoneSelectionMenu zoneMenu = new ZoneSelectionMenu(list, add);
                await Application.Current.MainPage.Navigation.PushModalAsync(zoneMenu);
                return;
            }
            PlaylistMenu((Playlist)e.Item, app, action, z);
        }

        public static async void PlaylistMenu(Playlist p, App context, string action, GeoZone z)
        {
            if (action.Equals("Edit"))
            {
                await Application.Current.MainPage.Navigation.PushAsync(new PlaylistPage(p));
            }
            else if(action.Equals("Play"))
            {
                await SongPlayer.Play(p);
            }
            else if(action.Equals("Cancel"))
            {
                //do nothing
            }
            else if(action.Equals("Delete"))
            {
                context.masterList.Remove(p);
                p = null;
            }
            else if(action.Equals("Assign to Zone"))
            {
                if (z != null)
                {
                    context.zoneList[z].Add(p);
                    p.onZoneBind(z);
                }
            }
            else if(action.Equals("Unassign from Zone"))
            {
                if (z != null) 
                { 
                    context.zoneList[z].Remove(p);
                    p.onZoneUnbind(z);
                }
            }
            else
            {
                p.rename(action);
            }
        }
    }
}