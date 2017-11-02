using MKCoolsoft.GPXLib;
using SharpKml.Base;
using System;
using System.Text;

namespace KmlBuilder
{
    public class Position
    {
        public Position() { }

        public Position(Wpt point)
        {
            Altitude = point.Ele;
            Latitude = point.Lat;
            Longitude = point.Lon;
            Time = new DateTimeOffset(point.Time);
        }

        public decimal? Altitude { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public DateTimeOffset? Time { get; set; }

        public Vector GetKmlVector()
        {
            Vector Result = new Vector();

            if ((Latitude.HasValue) && (Longitude.HasValue))
            {
                Result.Latitude = Convert.ToDouble(Latitude.Value);
                Result.Longitude = Convert.ToDouble(Longitude.Value);
            }

            if (Altitude.HasValue)
                Result.Altitude = Convert.ToDouble(Altitude.Value);

            return Result;
        }

        public void Debug(Action<string> write)
        {
            StringBuilder Buffer = new StringBuilder();

            Buffer.AppendFormat("[{0}] ==> ( {1} {2} )  Alt: {3}", 
                Time.HasValue ? Time.Value.ToString("yyyy-MM-dd HH:mm:ss zzz") : "<<NULL>>", 
                Latitude.HasValue ? Latitude.Value.ToString() : "<<NULL>>",
                Longitude.HasValue ? Longitude.Value.ToString() : "<<NULL>>", 
                Altitude.HasValue ? Altitude.Value.ToString() : "<<NULL>>");

            write(Buffer.ToString());
        }

    }
}
