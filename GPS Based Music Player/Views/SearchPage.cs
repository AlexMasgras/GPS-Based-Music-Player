using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace GPSBasedMusicPlayer
{
    public class SearchPage : ContentPage
    {
        Entry entry;
        ListView songs;
        Button searchButton;
        List<string> toDisplay;

        public SearchPage()
        {
            searchButton = new Button
            {
                Text = "OK",
                TextColor = Color.DarkOliveGreen,
                BackgroundColor = Color.LightGray,
                FontSize = 50,
                Margin = new Thickness(0),
            };
            searchButton.Clicked += updateList;

            songs = new ListView();
            songs.ItemsSource = toDisplay;
            songs.ItemTapped += (sender, args) => OnItemTapped(sender, args);
        }

        async void updateList(object sender, EventArgs args)
        {

        }
        async void OnItemTapped(object sender, EventArgs args)
        {
            MessagingCenter.Send(this, "a", sender);
            await Application.Current.MainPage.Navigation.PopModalAsync(true);
        }
    }
}
