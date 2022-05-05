using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace GPSBasedMusicPlayer
{
    public class Playlist : List<Song>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private string name { get; set; }
        private int position { get; set; }

        private List<string> refList;
        private List<GeoZone> boundZones;

        public Playlist(string name)
        {
            this.name = name;
            position = 0;
            refList = new List<string>();
            boundZones = new List<GeoZone>();
        }

        public new void Add(Song s)
        {
            base.Add(s);
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(0, s));
            refList.Add(s.GetRef());
        }

        public new void Remove(Song s)
        {
            refList.RemoveAt(base.IndexOf(s));
            base.Remove(s);
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, this));
        }

        public void onSongRename(Song s)
        {
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, s, s));
        }

        public void onZoneBind(GeoZone z)
        {
            boundZones.Add(z);
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
    }
}
