using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

using Newtonsoft.Json;

namespace GPSBasedMusicPlayer
{
    public class Playlist : INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public string name { get; set; }
        public int position { get; set; }

        private List<string> refList;
        private List<GeoZone> boundZones;
        public List<Song> songs;

        public Playlist(string name)
        {
            this.name = name;
            position = 0;
            refList = new List<string>();
            boundZones = new List<GeoZone>();
            songs = new List<Song>();
        }

        public Playlist(List<string> jsondata)
        {
            name = jsondata[0];
            jsondata.RemoveAt(0);

            position = int.Parse(jsondata[0]);
            jsondata.RemoveAt(0);

            songs = new List<Song>();
            refList = new List<string>();
            foreach (String t in jsondata)
            {
                Song s = JsonConvert.DeserializeObject<Song>(t);
                songs.Add(s);
                refList.Add(s.GetRef());
            }

            boundZones = new List<GeoZone>();
        }

        public void Add(Song s)
        {
            songs.Add(s);
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(0, s));
            refList.Add(s.GetRef());
        }

        public void Remove(Song s)
        {
            refList.RemoveAt(songs.IndexOf(s));
            songs.Remove(s);
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, this));
        }

        public int Size()
        {
            return songs.Count;
        }

        public List<Song> getSongs()
        {
            return new List<Song>(songs);
        }

        public void onSongRename(Song s)
        {
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, s, s));
        }

        public void onZoneBind(GeoZone z)
        {
            if (z.type.Equals("Circle") || z.type.Equals("Polygon"))
            {
                boundZones.Add(z);
            }
        }

        public void onZoneUnbind(GeoZone z)
        {
            if(boundZones.Contains(z))
            {
                boundZones.Remove(z);
            }
        }

        public List<GeoZone> getBoundZones()
        {
            return boundZones;
        }

        public void rename(string name)
        {
            this.name = name;
        }

        public int getPosition()
        {
            return position;
        }

        public void setPosition(int p)
        {
            position = p;
        }

        public List<string> getRefList()
        {
            return refList;
        }

        public override string ToString()
        {
            return name;
        }

        public string serialize()
        {
            List<string> serList = new List<string>();
            serList.Add(name);
            serList.Add(position.ToString());
            foreach(Song s in songs)
            {
                serList.Add(JsonConvert.SerializeObject(s));
            }

            return JsonConvert.SerializeObject(serList);
        }

        public bool functionallyEquals(Playlist other)
        {
            if(!name.Equals(other.name))
            {
                return false;
            }

            for(int i = 0; i < songs.Count; i++)
            {
                if(!songs[i].Equals(other.songs[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
