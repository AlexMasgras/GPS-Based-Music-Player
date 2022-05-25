using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;

namespace GPSBasedMusicPlayer
{
    public class MapPage : ContentPage
    {
        Button addButton;
        public MapPageViewModel cxt;
        public MapPage(MapPageViewModel model)
        {
            cxt = model;
            BindingContext = cxt;

            addButton = new Button
            {
                Text = "Add Zone",
                TextColor = Color.DarkOliveGreen,
                BackgroundColor = Color.LightGray,
                Margin = new Thickness(0)
            };
            addButton.SetBinding(Button.CommandProperty, nameof(MapPageViewModel.ZoneAddCommand));
            addButton.SetBinding(Button.TextProperty, nameof(MapPageViewModel.ButtonText));

            BackgroundColor = Color.DarkGray;


            var grid = new Grid
            {
                Margin = new Thickness(0, 0),

                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                RowDefinitions =
                {

                    new RowDefinition { Height = new GridLength(9, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                }
            };

            grid.Children.Add(cxt.map, 0, 0);
            grid.Children.Add(addButton, 0, 1);

            Content = grid;
        }
    }
}