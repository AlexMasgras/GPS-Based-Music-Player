using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Essentials;

namespace GPSBasedMusicPlayer
{ 
    public class Song
    {
        private SongType type { get; }
        private string reference { get; }

        private string name { get; set; }

        public Song(SongType type, string name, string refer)
        {
            this.type = type;
            this.name = name;
            if (type == SongType.FILE)
            {
                Uri temp = new Uri(Path.GetFullPath(refer));
                reference = temp.ToString();
            }
            else
            {
                reference = refer;
            }
        }

        public SongType GetType()
        {
            return type;
        }

        public string GetRef()
        {
            return reference;
        }

        public void rename(string s)
        {
            name = s;
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() != typeof(Song))
            {
                return false;
            }

            return type == ((Song)obj).GetType() && reference.Equals(((Song)obj).GetRef()) && name.Equals(((Song)obj).ToString());
        }

        public override string ToString()
        {
            return name;
        }
    }
}
