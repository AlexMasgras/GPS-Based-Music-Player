using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using MediaManager;

namespace GPSBasedMusicPlayer
{
    class SongPlayer
    {
        public static async Task Play(Song s)
        {
            await CrossMediaManager.Current.Play(s.GetRef());
        }

        public static async Task Play(Playlist p)
        {
            await CrossMediaManager.Current.Play(p.getRefList());
        }

        public static async Task Play(List<Playlist> l)
        {
            List<string> s = new List<string>();
            if(l.Count > 0)
            {
                foreach(Playlist p in l)
                {
                    if(p.Count > 0)
                    {
                        foreach(string t in p.getRefList())
                        {
                            s.Add(t);
                        }
                    }
                }
            }

            Random rnd = new Random();
            s = s.OrderBy(a => rnd.Next()).ToList();
            rnd = null;

            await CrossMediaManager.Current.Play(s);
        }

        public static void pause(Song s) { }

        public static void skip(Song s) { }
    }
}
