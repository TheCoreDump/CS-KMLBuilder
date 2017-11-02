using MKCoolsoft.GPXLib;
using System;
using System.IO;

namespace KmlBuilder.Gpx
{
    public class GpxLoader
    {

        private GpxLoader()
        {
        }


        public static GpxData GetGpxData(FileInfo gpxFile)
        {
            GPXLib sourceGpxData = new GPXLib();
            sourceGpxData.LoadFromFile(gpxFile.FullName);

            GpxData Result = new GpxData();

            sourceGpxData.TrkList.ForEach((t) => Result.Tracks.Add(GetTrack(t)));

            // Get waypoints?
            // Get routes?

            return Result;
        }



        public static GpxTrack GetTrack(Trk trackData)
        {
            GpxTrack Result = new GpxTrack();

            if (!string.IsNullOrEmpty(trackData.Name))
            {
                Result.Name = trackData.Name;
            }

            trackData.TrksegList.ForEach((s) => Result.Add(GetTrackSegment(s)));

            return Result;
        }


        public static GpxTrackSegment GetTrackSegment(Trkseg trackSegmentData)
        {
            GpxTrackSegment Result = new GpxTrackSegment();

            trackSegmentData.TrkptList.ForEach((w) => Result.Add(new Position(w)));

            return Result;
        }
    }
}
