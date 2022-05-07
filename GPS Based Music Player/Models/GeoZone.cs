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

        public static bool circlePoint(List<Position> centRad, Position gps)
        {
            return Xamarin.Forms.Maps.Distance.BetweenPositions(centRad[0], gps).Meters <
                Xamarin.Forms.Maps.Distance.BetweenPositions(centRad[0], centRad[1]).Meters;
        }


        //polyPoint algorithm from jeffreythompson.org/collisiondetection/poly-point.php
        public static bool polyPoint(List<Position> shape, double px, double py)
        {
            bool collision = false;

            // go through each of the vertices, plus
            // the next vertex in the list
            int next = 0;
            for (int current = 0; current < shape.Count; current++)
            {

                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == shape.Count) next = 0;

                // get the PVectors at our current position
                // this makes our if statement a little cleaner
                Position vc = shape[current];    // c for "current"
                Position vn = shape[next];       // n for "next"

                // compare position, flip 'collision' variable
                // back and forth
                if (((vc.Latitude >= py && vn.Latitude < py) || (vc.Latitude < py && vn.Latitude >= py)) &&
                     (px < (vn.Longitude - vc.Longitude) * (py - vc.Latitude) / (vn.Latitude - vc.Latitude) + vc.Longitude))
                {
                    collision = !collision;
                }
            }
            return collision;
        }
    }
}
