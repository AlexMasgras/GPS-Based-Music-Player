using System;
using System.Collections.Generic;
using System.Text;

namespace GPSBasedMusicPlayer
{
    public enum SongType
    {
        INTERNAL, FILE, SOUNDCLOUD, BANDCAMP, INVALID
    }

    static class SongTypeMethods
    {
        public static SongType ToSongType(string type)
        {
            if(type.ToLower().Equals("internal"))
            {
                return SongType.INTERNAL;
            }
            else if(type.ToLower().Equals("file"))
            {
                return SongType.FILE;
            }

            return SongType.INVALID;
        }
    }
}
