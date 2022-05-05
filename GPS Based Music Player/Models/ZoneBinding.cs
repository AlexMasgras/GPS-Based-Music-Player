using System;
using System.Collections.Generic;
using System.Text;

namespace GPSBasedMusicPlayer
{
    public class ZoneBinding
    {
        private List<Playlist> lists;
        private List<Song> songs;
        private List<string> songRefs;

        public ZoneBinding()
        {
            lists = new List<Playlist>();
            songs = new List<Song>();
            songRefs = new List<string>();
        }

        public List<Playlist> getLists()
        {
            return lists;
        }

        public List<Song> getSongs()
        {
            return songs;
        }

        public List<string> getRefs()
        {
            return songRefs;
        }

        public void addList(Playlist p)
        {
            lists.Add(p);
            if (p.Count > 0)
            {
                foreach (Song s in p)
                {
                    songs.Add(s);
                    songRefs.Add(s.GetRef());
                }
            }
        }

        public void removeList(Playlist p)
        {
            lists.Remove(p);
            foreach(Song s in p)
            {
                if(songs.Contains(s) && songRefs.Contains(s.GetRef()))
                {
                    songs.Remove(s);
                    songRefs.Remove(s.GetRef());
                }
            }
        }
    }
}
