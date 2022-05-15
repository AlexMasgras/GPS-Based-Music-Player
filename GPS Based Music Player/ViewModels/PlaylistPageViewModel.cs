using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Essentials;
using System.IO;

namespace GPSBasedMusicPlayer
{
    public class PlaylistPageViewModel : ContentPage
    {
        public PlaylistPageViewModel(Playlist list)
        {
            AddNew = new Command(() =>
            {
                ButtonClicked(list);
            });

        }

        async void ButtonClicked(Playlist list)
        {
            //try
            //{
                SongType type = SongTypeMethods.ToSongType(await Application.Current.MainPage.DisplayActionSheet("Type of song?", "Cancel", null, "File", "Soundcloud", "Bandcamp"));
                var result = await FilePicker.PickAsync();
                string name = await Application.Current.MainPage.DisplayPromptAsync("New Song", "Name of song?");

                if(result != null)
                {
                    list.Add(new Song(type, name, await StoreAudioFile(result.OpenReadAsync(), name)));
                }

            //}
#pragma warning disable CS0168 // Variable is declared but never used
            //catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            //{
                // The user canceled or something went wrong
            //}
        }

        public Command AddNew { get; }

        public static async Task<string> StoreAudioFile(Task<Stream> inputStream, string name)
        {
            string folder = FileSystem.AppDataDirectory;

            string videoFile = Path.Combine(folder, name +".mp3"); ;
            if (!File.Exists(videoFile))
            {
                using (FileStream outputStream = File.Create(videoFile))
                {
                    await inputStream.Result.CopyToAsync(outputStream);
                }

            }

            return videoFile;
        }

        public static async Task SongMenu(Song s, Playlist p, string action)
        {
            if (action.Equals("Yeet"))
            {
                p.Remove(s);
            }
            else if (action.Equals("Play"))
            {
                await SongPlayer.Play(s);
            }
            else if (action.Equals("Cancel"))
            {
                //do nothing
            }
            else
            {
                s.rename(action);
                p.onSongRename(s);
            }
        }
    }
}