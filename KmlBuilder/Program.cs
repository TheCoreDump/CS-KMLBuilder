using KmlBuilder.Exif;
using KmlBuilder.Gpx;
using KmlBuilder.Kml;
using MKCoolsoft.GPXLib;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using KmlPoint = SharpKml.Dom.Point;


namespace KmlBuilder
{
    public class Program
    {

        private static System.TimeSpan CameraClockOffset = new System.TimeSpan(0, 0, 209);

        public static void Main(string[] args)
        {
            // TestGPX();
            TestExif2();
            // TestKml();
            // TestWriteKml();

            //Final(args);
        }


        public static void Final(string[] args)
        {
            // First, get the gpx data
            FileInfo gpxFile = new FileInfo("C:\\Temp\\KmlTest\\Flory Preserve\\GPX\\Flory Preserve.gpx");
            GpxData gpxData = GpxLoader.GetGpxData(gpxFile);

            List<Position> gpsPositionData = new List<Position>();
            gpxData.Tracks.ForEach((t) => t.ForEach((ts) => ts.ForEach((p) => gpsPositionData.Add(p))));



            // Second, get the photo information
            DirectoryInfo photoDirectory = new DirectoryInfo("C:\\Temp\\KMLTest\\Flory Preserve\\Pictures");

            Dictionary<FileInfo, PhotoInfo> PhotoInformation = new Dictionary<FileInfo, PhotoInfo>();

            foreach (FileInfo tmpFile in photoDirectory.GetFiles("*.jpg"))
            {
                PhotoInfo tmpPhotoInfo = MyExifReader.Create(tmpFile).GetPhotoInfo(CameraClockOffset);
                FindGpsInfo(tmpPhotoInfo, gpsPositionData);

                PhotoInformation.Add(tmpFile, tmpPhotoInfo);
            }


            foreach (KeyValuePair<FileInfo, PhotoInfo> tmpPhotoInfo in PhotoInformation)
            {
                tmpPhotoInfo.Value.Debug((s) => Console.WriteLine(s));
            }

            FileInfo targetFile = new FileInfo("C:\\Temp\\KMLTest\\Flory Preserve\\Flory Preserve V3.kml");

            if (targetFile.Exists)
                targetFile.Delete();

            KmlPicturePlacemarkWriter.Write(new FileInfo("C:\\Temp\\KMLTest\\Flory Preserve\\Flory Preserve V3.kml"), gpxData, PhotoInformation.Values.ToList());
            //KmlWriter.Write(new FileInfo("C:\\Temp\\KMLTest\\First.kml"), gpsPositionData, PhotoInformation.Values.ToList());
        }


        public static void FindGpsInfo(PhotoInfo photo, List<Position> gpxPositionData)
        {
            if (photo == null)
                return;

            if (!photo.DateTimeDigitized.HasValue)
                return;

            Position closestPosition = gpxPositionData[0];

            gpxPositionData.ForEach((p) =>
            {
                decimal currentDiff = Math.Abs(Convert.ToDecimal(photo.DateTimeDigitized.Value.Subtract(closestPosition.Time.Value).TotalSeconds));
                decimal newDiff = Math.Abs(Convert.ToDecimal(photo.DateTimeDigitized.Value.Subtract(p.Time.Value).TotalSeconds));

                if (newDiff < currentDiff)
                    closestPosition = p;
            });

            photo.ClosestGPSPoint = closestPosition;
        }


        public static void TestWriteKml()
        {
            Console.WriteLine("Creating a point at 37.42052549 latitude and -122.0816695 longitude.\n");

            // This will be used for the placemark
            KmlPoint point = new KmlPoint();
            point.Coordinate = new Vector(37.42052549, -122.0816695);

            Placemark placemark = new Placemark();
            placemark.Name = "Cool Statue";
            placemark.Geometry = point;

            // This is the root element of the file
            Folder RootFolder = new Folder() { Id = "RootFolderId", Name = "RootFolderName" };
            

            Style CampfireStyle = IconStyleMapBuilder.GetStyle("campfire", "http://maps.google.com/mapfiles/kml/shapes/campfire.png");

            Document RootDocument = new Document();
            KmlFile tmpFile = KmlFile.Create(RootDocument, false);

            RootDocument.AddStyle(CampfireStyle.Clone());

            //placemark.AddStyle(StyleResolver.CreateResolvedStyle(RootDocument, tmpFile, StyleState.Normal));
            placemark.AddStyle(CampfireStyle);

            Folder Day1Folder = new Folder();
            RootFolder.AddFeature(Day1Folder);

            Day1Folder.Id = "Day1RootFolder";
            Day1Folder.Name = "Day 1";
            Day1Folder.Description = new Description() { Text = "First day of our hike in Shenandoah National Park" };

            //Day1Folder.AddFeature(StyleResolver.InlineStyles(placemark));
            RootDocument.AddFeature(RootFolder);
            Day1Folder.AddFeature(StyleResolver.SplitStyles(placemark));

            //KmlFile finalFile = KmlFile.Create(StyleResolver.SplitStyles(placemark), false);


            using (FileStream FS = new FileStream("C:\\Temp\\KMLTest\\First.kml", FileMode.Create, FileAccess.Write))
            {
                tmpFile.Save(FS);
            }

            /*
            KmlParser parser = new KmlParser();
            parser.ParseString(serializer.Xml, true);

            kml = (Kml)parser.Root;
            placemark = (Placemark)kml.Feature;
            point = (SharpKml.Dom.Point)placemark.Geometry;

            Console.WriteLine("Latitude:{0} Longitude:{1}", point.Coordinate.Latitude, point.Coordinate.Longitude);
            */
        }

        public static void TestExif2()
        {
            DirectoryInfo photoDirectory = new DirectoryInfo("C:\\Temp\\KMLTest\\Pictures");

            Dictionary<FileInfo, PhotoInfo> PhotoInformation = new Dictionary<FileInfo, PhotoInfo>();
            List<Position> PositionInformation = GetPositionInformation();



            foreach (FileInfo tmpFile in photoDirectory.GetFiles("BigEndian.jpg"))
            {
                MyExifReader reader = MyExifReader.Create(tmpFile);

                PhotoInfo info = reader.GetPhotoInfo(CameraClockOffset);


                Console.WriteLine("Camera Offset (seconds): {0}s", info.DateTimeDigitized.Value.Subtract(info.PositionInfo.Time.Value).TotalSeconds);

                info.Debug(Console.WriteLine);

                /*
                Console.WriteLine("*** Open File: {0} ***", tmpFile.Name);

                BitmapDecoder decoder = JpegBitmapDecoder.Create(tmpFile.OpenRead(), BitmapCreateOptions.None, BitmapCacheOption.None);
                

                foreach (BitmapFrame bf in decoder.Frames)
                {
                    Console.WriteLine("Frame");
                    BitmapMetadata bm = bf.Metadata as BitmapMetadata;

                    List<RawMetadataItem> Results = new List<RawMetadataItem>();

                    CaptureMetadata(Results, bm, string.Empty);


                    Results.ForEach((rmd) => Console.WriteLine("Location: {0}  Value : {1} ({2})", rmd.location, rmd.GetReadableValue(), rmd.value == null ? "<<NULL>>" : rmd.value.GetType().ToString()));
                }
                */
            }
        }

        private static void CaptureMetadata(List<RawMetadataItem> Result, ImageMetadata imageMetadata, string query)
        {
            BitmapMetadata bitmapMetadata = imageMetadata as BitmapMetadata;

            if (bitmapMetadata != null)
            {
                foreach (string relativeQuery in bitmapMetadata)
                {
                    string fullQuery = query + relativeQuery;
                    object metadataQueryReader = bitmapMetadata.GetQuery(relativeQuery);

                    RawMetadataItem metadataItem = new RawMetadataItem();

                    metadataItem.location = fullQuery;
                    metadataItem.value = metadataQueryReader;

                    Result.Add(metadataItem);

                    BitmapMetadata innerBitmapMetadata = metadataQueryReader as BitmapMetadata;

                    if (innerBitmapMetadata != null)
                    {
                        CaptureMetadata(Result, innerBitmapMetadata, fullQuery);
                    }
                }
            }
        }


        /*
        public static string GetValue(ExifItem item)
        {
            if (item.Value == null)
                return "<<NULL>>";

            if (item.Value is string)
                return item.Value.ToString();

            if (item.Value is int)
                return Convert.ToInt32(item.Value).ToString();

            if (item.Value is URational)
            {
                URational urValue = (URational) item.Value;
                return urValue.Denominator == 0 ? "<<DIV/0>>" : (Convert.ToDecimal(urValue.Numerator) / Convert.ToDecimal(urValue.Denominator)).ToString();
            }

            return item.ToString();
        }
        */

        public static decimal? ConvertLatLon(double[] values, string refValue)
        {
            if (values != null)
            {
                return Convert.ToDecimal((values[0] + (values[1] / 60.0) + (values[2] / 3600.0)) * (((refValue == "W") || (refValue == "S")) ? -1.0 : 1.0));
            }

            return null;
        }

        public static string FormatLatLon(double[] values, string refValue)
        {
            if (values != null)
                return string.Format("{0}° {1}' {2}\" {3}", values[0], values[1], values[2], refValue);

            return string.Empty;
        }


        public static List<Position> GetPositionInformation()
        {
            GPXLib tmpGPX = new GPXLib();
            List<Position> Result = new List<Position>();

            tmpGPX.LoadFromFile("C:\\Temp\\KMLTest\\GPX\\Shenandoah Day 2.gpx");

            tmpGPX?.TrkList[0]?.TrksegList[0]?.TrkptList?.ForEach((w) => Result.Add(new Position(w)));

            return Result;
        }

        private static void DebugMessage(string format, params object[] formatValues)
        {
            Console.WriteLine(format, formatValues);
        }
    }

    public class RawMetadataItem
    {
        public String location;
        public Object value;

        public string GetReadableValue()
        {
            if (value == null)
                return null;

            if (converters.ContainsKey(value.GetType()))
            {
                return converters[value.GetType()](value);
            }
            else
            {
                return Convert.ToString(value);
            }
        }

        private static Dictionary<Type, Func<object, string>> converters = new Dictionary<Type, Func<object, string>>()
        {
            { typeof(UInt16), (o) => Convert.ToString(o) },
            { typeof(BitmapMetadataBlob), (o) => DescribeMetadataBlob(o as BitmapMetadataBlob) }
        };

        private static string DescribeMetadataBlob(BitmapMetadataBlob blob)
        {
            if (blob == null)
                return "Blob is <<NULL>>";

            return string.Format("Blob length: {0}", blob.GetBlobValue().Length.ToString());
        }
    }

}
