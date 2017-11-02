using KmlBuilder.Gpx;
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
    public class KmlPicturePlacemarkWriter
    {

        private KmlPicturePlacemarkWriter() { }

        public static void Write(FileInfo outputFile, GpxData gpxData, List<PhotoInfo> photoInfo)
        {
            SharpKml.Dom.Kml ResultKml = new SharpKml.Dom.Kml();

            ResultKml.AddNamespacePrefix(KmlNamespaces.GX22Prefix, KmlNamespaces.GX22Namespace);

            Document RootDocument = new Document() { Description = new Description() { Text = "Flory Preserve" }, Id = "FloryPreserveRootId", Name = "Flory Preserve" };

            // Add the style for the photo icon
            BuildCommonStyles((s) => RootDocument.AddStyle(s));

            foreach (GpxTrack tmpGpxTrack in gpxData.Tracks)
            {
                string TrackName = tmpGpxTrack.Name;
                string TrimmedTrackName = tmpGpxTrack.Name.Replace(" ", "");

                Folder trackFolder = new Folder() { Name = TrackName, Id = string.Format("{0}FolderId", tmpGpxTrack.Name.Replace(" ", "")) };

                IOrderedEnumerable<Position> gpxDataSorted = tmpGpxTrack.GetAllPositions().OrderBy(k => k.Time);

                Track newTrack = new Track()
                {
                    AltitudeMode = SharpKml.Dom.AltitudeMode.ClampToGround,
                    Id = TrimmedTrackName + "Id"
                };


                foreach (Position tmpPosition in gpxDataSorted)
                {
                    newTrack.AddWhen(tmpPosition.Time.Value.UtcDateTime);
                    newTrack.AddCoordinate(new Vector(Convert.ToDouble(tmpPosition.Latitude.Value), Convert.ToDouble(tmpPosition.Longitude.Value)));
                }


                // Build the style for the line.
                Style trackStyle = GetTrackStyle(TrimmedTrackName + "StyleId", tmpGpxTrack);
                RootDocument.AddStyle(trackStyle);


                Placemark trackPlaceMark = new Placemark()
                {
                    Geometry = newTrack,
                    Name = TrackName,
                    StyleUrl = new Uri("#" + trackStyle.Id, UriKind.Relative)
                };

                trackFolder.AddFeature(trackPlaceMark);
                RootDocument.AddFeature(trackFolder);
            }


            Folder PhotoFolder = new Folder() { Name = "Photos", Id = "PhotoFolder" };

            foreach (PhotoInfo tmpPhoto in photoInfo)
            {
                Placemark photoPlacemark = new Placemark()
                {
                    Name = tmpPhoto.FileObject.Name,
                    StyleUrl = new Uri("#sm_PhotoStyleId", UriKind.Relative),
                    Geometry = tmpPhoto.GetFinalPoint(),
                    Description = new Description() { Text = string.Format("<![CDATA[<img style=\"max-width:500px;\" src=\"file:///{0}\">]]>", tmpPhoto.FileObject.FullName.Replace('\\', '/')) }
                };

                photoPlacemark.Name = tmpPhoto.FileObject.Name;
                PhotoFolder.AddFeature(photoPlacemark);
            }



            RootDocument.AddFeature(PhotoFolder);

            ResultKml.Feature = RootDocument;

            KmlFile outputKmlFile = KmlFile.Create(ResultKml, false);

            outputKmlFile.Save(outputFile.OpenWrite());
        }

        public static void BuildCommonStyles(Action<StyleSelector> addStyle)
        {
            Style normalStyle = new Style()
            {
                Id = "sn_PhotoStyleId",
                Icon = new IconStyle()
                {
                    Scale = 1.2,
                    Icon = new IconStyle.IconLink(new Uri("http://maps.google.com/mapfiles/kml/shapes/camera.png")),
                    Hotspot = new Hotspot() { X = 0.5, Y = 0, XUnits = Unit.Fraction, YUnits = Unit.Fraction }
                }
            };


            Style highlightStyle = new Style()
            {
                Id = "sh_PhotoStyleId",
                Icon = new IconStyle()
                {
                    Scale = 1.4,
                    Icon = new IconStyle.IconLink(new Uri("http://maps.google.com/mapfiles/kml/shapes/camera.png")),
                    Hotspot = new Hotspot() { X = 0.5, Y = 0, XUnits = Unit.Fraction, YUnits = Unit.Fraction }
                }                
            };



            StyleMapCollection Result = new StyleMapCollection()
            {
                Id = "sm_PhotoStyleId"
            };

            Result.Add(new Pair()
            {
                StyleUrl = new Uri("#sn_PhotoStyleId", UriKind.Relative),
                State = StyleState.Normal
            });

            Result.Add(new Pair()
            {
                StyleUrl = new Uri("#sh_PhotoStyleId", UriKind.Relative),
                State = StyleState.Highlight
            });

            addStyle(Result);
            addStyle(normalStyle);
            addStyle(highlightStyle);
        }

        private static Style GetTrackStyle(string styleId, GpxTrack data)
        {
            Style Result = new Style();
            Result.Id = styleId;

            Color32 LineColor;

            switch (data.Name)
            {
                case "Deer Run":
                    LineColor = new Color32(0xFF, 0xFF, 0x00, 0x00);
                    break;
                case "Rock Ridge":
                    LineColor = new Color32(0xFF, 0x00, 0x00, 0xFF);
                    break;
                case "Flory Trail":
                    LineColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
                    break;
                default:
                    LineColor = new Color32(0xFF, 0x00, 0xFF, 0x00);
                    break;
            }


            Result.Line = new LineStyle()
            {
                Color = LineColor,
                ColorMode = ColorMode.Normal,
                LabelVisibility = false,
                Width = 4
            };

            return Result;
        }
    }
}
