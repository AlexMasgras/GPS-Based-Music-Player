namespace GPSBasedMusicPlayer
{
    using Xamarin.Forms.Maps;
    using System.Collections.Generic;
    public class GeoZone
    {
        public string name { get; set; }
        public List<Position> coords { get; set; }

        public string type { get; set; }

        public GeoZone(string name, string type, List<Position> coords)
        {
            this.name = name;
            this.coords = coords;
            this.type = type;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
