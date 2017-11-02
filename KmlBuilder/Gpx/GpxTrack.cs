using System.Collections.Generic;

namespace KmlBuilder.Gpx
{
    public class GpxTrack : List<GpxTrackSegment>
    {
        public string Name { get; set; }


        public List<Position> GetAllPositions()
        {
            List<Position> Result = new List<Position>();

            ForEach(ts => ts.ForEach(p => Result.Add(p)));

            return Result;
        }
    }
}
