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
        MusicPageViewModel baseModel;
        public MusicPage(MusicPageViewModel model)
        {
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
            playlists.ItemsSource = model.getMaster;
            playlists.ItemTapped += model.OnTap;

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
    }
}