using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace GPSBasedMusicPlayer
{
    public class ZoneSelectionMenu : ContentPage
    {
        Label selectionLabel;
        Button okButton;
        Picker picker;
        public ZoneSelectionMenu(List<GeoZone> list, bool add)
        {
            selectionLabel = new Label
            {
                Text = "Select Geo Zone",
                FontSize = 40,
                Margin = new Thickness(1)
            };

            okButton = new Button
            {
                Text = "OK",
                TextColor = Color.DarkOliveGreen,
                BackgroundColor = Color.LightGray,
                FontSize = 50,
                Margin = new Thickness(0),
            };
            okButton.SetBinding(Button.CommandProperty, nameof(ZoneSelectionMenuModel.returnItem));
            okButton.Clicked += (sender, args) => OnButtonClicked(sender, args);

            picker = new Picker
            {
                Title = "Zone",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                ItemsSource = list
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
                    new RowDefinition { Height = new GridLength(0.5, GridUnitType.Star) }
                }
            };

            grid.Children.Add(selectionLabel, 0, 0);
            grid.Children.Add(picker, 0, 1);
            grid.Children.Add(okButton, 0, 2);

            Content = grid;
        }

        async void OnButtonClicked(object sender, EventArgs args)
        {
            MessagingCenter.Send(this, "a", (GeoZone)picker.SelectedItem);
            await Application.Current.MainPage.Navigation.PopModalAsync(true);
        }
    }

}