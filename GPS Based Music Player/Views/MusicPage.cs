using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace GPSBasedMusicPlayer
{
    public class MusicPage : ContentPage
    {
        Button newButton;
        ListView playlists;
        MainPageViewModel baseModel;
        public MusicPage(MainPageViewModel model)
        {
            BindingContext = new MusicPageViewModel(model);
            baseModel = model;

            BackgroundColor = Color.DarkGray;

            newButton = new Button
            {
                Text = "Add Playlist",
                TextColor = Color.DarkOliveGreen,
                BackgroundColor = Color.LightGray,
                Margin = new Thickness(0)
            };
            newButton.SetBinding(Button.CommandProperty, nameof(MusicPageViewModel.AddNew));

            playlists = new ListView();
            playlists.ItemsSource = model.masterList;
            playlists.ItemTapped += OnTap;

            var grid = new Grid
            {
                Margin = new Thickness(0, 0),

                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(7, GridUnitType.Star) }
                }
            };

            grid.Children.Add(newButton, 0, 0);
            grid.Children.Add(playlists, 0, 1);

            Content = grid;
        }
        async void OnTap(object sender, ItemTappedEventArgs e)
        {
            string action = await DisplayActionSheet("Playlist: " + e.Item.ToString(), "Cancel", "Delete", "Edit", "Rename", "Play", "Assign to Zone", "Unassign from Zone");
            GeoZone z = null;
            if(action.Equals("Assign to Zone") || action.Equals("Unassign from Zone"))
            {
                MessagingCenter.Subscribe<ZoneSelectionMenu, GeoZone>(this, "a", (send, arg) =>
                {
                    MusicPageViewModel.PlaylistMenu((Playlist)e.Item, baseModel, action, arg);
                    MessagingCenter.Unsubscribe<ZoneSelectionMenu, GeoZone>(this, "a");
                });

                bool add = action.Equals("Assign to Zone");
                List<GeoZone> list = add ? new List<GeoZone>(baseModel.zoneList.Keys.ToList()) : ((Playlist)e.Item).getBoundZones();

                if(add)
                {
                    List<GeoZone> toDisplay = new List<GeoZone>();
                    foreach (GeoZone zone in list)
                    {
                        if(zone.ToString() != null && !((Playlist)e.Item).getBoundZones().Contains(zone))
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
            MusicPageViewModel.PlaylistMenu((Playlist)e.Item, baseModel, action, z);
        }
    }
}