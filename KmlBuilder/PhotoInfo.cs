using KmlBuilder.Exif;
using SharpKml.Base;
using SharpKml.Dom;
using System;
using System.IO;
using System.Text;

namespace KmlBuilder
{
    public class PhotoInfo
    {

        public PhotoInfo()
        { }

        public FileInfo FileObject { get; set; }

        public DateTimeOffset? DateTimeDigitized { get; set; }

        public Position PositionInfo { get; set; }

        public Position ClosestGPSPoint { get; set; }


        public Point GetFinalPoint()
        {
            Point Result = new Point();

            Result.AltitudeMode = AltitudeMode.ClampToGround;

            if (PositionInfo != null)
            {
                Result.Coordinate = PositionInfo.GetKmlVector();
            }
            else if (ClosestGPSPoint != null)
            {
                Result.Coordinate = ClosestGPSPoint.GetKmlVector();
            }

            return Result;
        }

        public void Debug(Action<string> write)
        {
            StringBuilder Buffer = new StringBuilder();

            Buffer.AppendFormat("File: {1}{0}", Environment.NewLine, FileObject.FullName);
            Buffer.AppendFormat("Photo Date Time: {1}{0}", Environment.NewLine, DateTimeDigitized.HasValue ? DateTimeDigitized.Value.ToString("yyyy-MM-dd HH:mm:ss zzz") : "<<NULL>>");

            if (PositionInfo != null)
                PositionInfo.Debug((s) => Buffer.AppendLine(s));
            else
                Buffer.AppendLine("Position Info: <<NULL>>");

            if (ClosestGPSPoint != null)
                ClosestGPSPoint.Debug((s) => Buffer.AppendLine(s));
            else
                Buffer.AppendLine("Closest GPS Info: <<NULL>>");

            write(Buffer.ToString());
        }
    }
}
