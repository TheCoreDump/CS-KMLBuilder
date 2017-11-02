using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Dom.GX;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KmlBuilder.Kml
{
    public class KmlWriter
    {
        private KmlWriter() { }



        public static void Write(FileInfo outputFile, List<Position> gpxPositionInfo, List<PhotoInfo> photoInfo)
        {
            SharpKml.Dom.Kml ResultKml = new SharpKml.Dom.Kml();

            ResultKml.AddNamespacePrefix(KmlNamespaces.GX22Prefix, KmlNamespaces.GX22Namespace);

            Document RootDocument = new Document() { Description = new Description() { Text = "Test KML File" }, Id = "RootFolderId", Name = "RootFolderName" };

            var groupings = gpxPositionInfo.GroupBy((p) => p.Time.HasValue ? p.Time.Value.Date : (DateTime?)null);

            foreach (var tmpGrouping in groupings)
            {
                IOrderedEnumerable<Position> gpxDataSorted = tmpGrouping.OrderBy((k) => k.Time);

                // Write the track data:
                Track tmpTrack = new Track()
                {
                    AltitudeMode = SharpKml.Dom.AltitudeMode.ClampToGround,
                    Id = tmpGrouping.Key.HasValue ? string.Format("Track{0}", tmpGrouping.Key.Value.ToString("yyyy-MM-dd")) : "NULLDate",
                };


                foreach (Position tmpPosition in gpxDataSorted)
                {
                    tmpTrack.AddWhen(tmpPosition.Time.Value.UtcDateTime);
                    tmpTrack.AddCoordinate(new Vector(Convert.ToDouble(tmpPosition.Latitude.Value), Convert.ToDouble(tmpPosition.Longitude.Value)));
                }


                Placemark trackPlaceMark = new Placemark()
                {
                    Geometry = tmpTrack,
                    Name = tmpGrouping.Key.HasValue ? string.Format("Track - {0}", tmpGrouping.Key.Value.ToString("yyyy-MM-dd")) : "Unknown Date",
                };

                RootDocument.AddFeature(trackPlaceMark);
            }


            ResultKml.Feature = RootDocument;

            KmlFile outputKmlFile = KmlFile.Create(ResultKml, false);

            outputKmlFile.Save(outputFile.OpenWrite());
        }

    }
}
