using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace GPSBasedMusicPlayer
{
    public class MainPage : ContentPage
    {
        Image logoImage;
        Label songLabel;
        Label timeLabel;
        Label zoneLabel;
        Button mapButton;
        Button musicButton;
        public MainPage()
        {
            BackgroundColor = Color.DarkGray;

            BindingContext = new MainPageViewModel();

            logoImage = new Image
            {
                Source = "LogoAlpha.png"
            };

            mapButton = new Button
            {
                Text = "MAP",
                TextColor = Color.DarkOliveGreen,
                BackgroundColor = Color.LightGray,
                Margin = new Thickness(0)
            };
            mapButton.SetBinding(Button.CommandProperty, nameof(MainPageViewModel.MapMenuCommand));

            musicButton = new Button
            {
                Text = "PLAYLISTS",
                TextColor = Color.DarkOliveGreen,
                BackgroundColor = Color.LightGray,
                Margin = new Thickness(0)
            };
            musicButton.SetBinding(Button.CommandProperty, nameof(MainPageViewModel.MusicMenuCommand));

            songLabel = new Label
            {
                Text = "Now Playing: Test Song",
                FontSize = 20,
                Margin = new Thickness(1)
            };
            songLabel.SetBinding(Label.TextProperty, nameof(MainPageViewModel.CurrentSong));

            zoneLabel = new Label
            {
                Text = "Zone: ",
                FontSize = 20,
                Margin = new Thickness(1)
            };

            timeLabel = new Label
            {
                Text = "Current Time: 12:00 AM 1/1/1970",
                FontSize = 20,
                Margin = new Thickness(1)
            };

            var grid = new Grid
            {
                Margin = new Thickness(0, 0),

                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(2.5, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(0.2, GridUnitType.Star) }
                }
            };

            grid.Children.Add(logoImage, 0, 1);
            grid.Children.Add(mapButton, 0, 0);
            grid.Children.Add(musicButton, 0, 2);
            grid.Children.Add(songLabel, 0, 3);
            grid.Children.Add(zoneLabel, 0, 4);
            grid.Children.Add(timeLabel, 0, 5);

            Content = grid;
        }
    }
}