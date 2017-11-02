using System;

namespace KmlBuilder.Exif
{
    public class CanonTimeInfo
    {
        public CanonTimeInfo(ExifDataSource dataSource, IFDEntry timeInfoEntry)
        {
            int Offset = dataSource.ToInt(timeInfoEntry.ValueBytes);
        }

        public DateTimeOffset ImageDateTime { get; set; }

    }
}
