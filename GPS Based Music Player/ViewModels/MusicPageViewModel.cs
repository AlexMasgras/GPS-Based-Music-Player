using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace GPSBasedMusicPlayer
{
    public class MusicPageViewModel : ContentPage
    {
        public MusicPageViewModel(MainPageViewModel model)
        {
            AddNew = new Command( async () =>
            {
                string result = await Application.Current.MainPage.DisplayPromptAsync("Playlist Creation", "Name your Playlist");
                model.masterList.Add(new Playlist(result));
            });
            
        }

        public Command AddNew { get; }

        public static async void PlaylistMenu(Playlist p, MainPageViewModel context, string action, GeoZone z)
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