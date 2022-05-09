using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace GPSBasedMusicPlayer
{
    public class PlaylistPage : ContentPage
    {
        Button newButton;
        ListView songs;
        public PlaylistPage(Playlist list)
        {
            BindingContext = new PlaylistPageViewModel(list);

            BackgroundColor = Color.DarkGray;

            newButton = new Button
            {
                Text = "Add Song",
                TextColor = Color.DarkOliveGreen,
                BackgroundColor = Color.LightGray,
                Margin = new Thickness(0)
            };
            newButton.SetBinding(Button.CommandProperty, nameof(MusicPageViewModel.AddNew));

            songs = new ListView();
            songs.ItemsSource = list.getSongs();
            songs.ItemTapped += OnTap;

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
            grid.Children.Add(songs, 0, 1);

            Content = grid;
        }
        async void OnTap(object sender, ItemTappedEventArgs e)
        {
            string action = await DisplayActionSheet("Song: " + e.Item.ToString(), "Cancel", "Yeet", "Rename");
            if(action.Equals("Rename"))
            {
                action = await DisplayPromptAsync("Rename Playlist", "New Name: ");
            }
            PlaylistPageViewModel.SongMenu((Song)e.Item, (Playlist)songs.ItemsSource, action);
        }
    }
}