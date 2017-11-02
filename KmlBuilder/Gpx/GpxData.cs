using System;
using System.Collections.Generic;

namespace KmlBuilder.Gpx
{
    public class GpxData
    {
        public GpxData()
        {
            Tracks = new List<GpxTrack>();
        }

        public List<GpxTrack> Tracks { get; set; }
    }
}
