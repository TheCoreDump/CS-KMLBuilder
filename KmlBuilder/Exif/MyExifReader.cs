using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


namespace KmlBuilder.Exif
{
    public class MyExifReader : IDisposable, IPhotoInfoFactory
    {

        private MyExifReader()
        {
        }

        private FileInfo ImageFile { get; set; }

        private ExifDataSource Source { get; set; }

        public Dictionary<ushort, IFDEntry> ifdCatalog { get; set; }

        public Dictionary<ushort, IFDEntry> gpsCatalog { get; set; }

        public Dictionary<ushort, IFDEntry> exifCatalog { get; set; }

        public Dictionary<ushort, IFDEntry> makerNotesCatalog { get; set; }


        public static MyExifReader Create(FileInfo imageFile)
        {
            MyExifReader Result = new MyExifReader();

            Result.ImageFile = imageFile;
            Result.Read();

            return Result;
        }

        public void Read()
        {
            Stream imageStream = ImageFile.OpenRead();

            Source = new ExifDataSource();
            Source.Initialize(imageStream);

            ifdCatalog = CatalogueIFD(Source, Source.IFDOffset, typeof(ExifTags2));

            if (ifdCatalog.ContainsKey(0x8825))
                gpsCatalog = CatalogueIFD(Source, Convert.ToInt64(ifdCatalog[0x8825].Value), typeof(GPSTags));
                //gpsCatalog = CatalogueIFD(Source, Source.ToInt(ifdCatalog[0x8825].ValueBytes), typeof(GPSTags));
            else
                gpsCatalog = new Dictionary<ushort, IFDEntry>();

            if (ifdCatalog.ContainsKey(0x8769))
                exifCatalog = CatalogueIFD(Source, Convert.ToInt64(ifdCatalog[0x8769].Value), typeof(ExifTags2));
            else
                exifCatalog = new Dictionary<ushort, IFDEntry>();

            if (exifCatalog.ContainsKey(0x927c))
            {
                long MakerTagOffset = Source.ToUint(exifCatalog[0x927c].OriginalValueBytes);

                Source.PushLittleEndian(true);

                makerNotesCatalog = CatalogueIFD(Source, MakerTagOffset, typeof(CanonMakerTags));

                Source.PopLittleEndian();
            }
            else
                makerNotesCatalog = new Dictionary<ushort, IFDEntry>();

            //DumpCatalogsToDebug();
        }


        private void DumpCatalogsToDebug()
        {
            foreach (KeyValuePair<ushort, IFDEntry> entry in ifdCatalog)
            {
                DebugMessage("IFD: {0}", entry.Value.ToString());
            }


            foreach (KeyValuePair<ushort, IFDEntry> entry in gpsCatalog)
            {
                DebugMessage("GPS: {0}", entry.Value.ToString());
            }


            foreach (KeyValuePair<ushort, IFDEntry> entry in exifCatalog)
            {
                if (entry.Key != ExifTags2.MakerNotes.GetHashCode())
                    DebugMessage("Exif: {0}", entry.Value.ToString());
            }


            foreach (KeyValuePair<ushort, IFDEntry> entry in makerNotesCatalog)
            {
                DebugMessage("MakerNotes: {0}", entry.Value.ToString());
            }
        }


        private Dictionary<ushort, IFDEntry> CatalogueGPS(ExifDataSource source, IFDEntry gpsEntry)
        {
            Dictionary<ushort, IFDEntry> Results = new Dictionary<ushort, IFDEntry>();

            source.PushAndSeek(source.ToUint(gpsEntry.ValueBytes));

            ushort entryCount = source.ReadUShort();

            for (ushort currentEntry = 0; currentEntry < entryCount; currentEntry++)
            {
                IFDEntry entry = new IFDEntry();

                entry.TagId = source.ReadUShort();
                entry.Type = source.ReadUShort();
                entry.Count = source.ReadUint();

                entry.OriginalValueBytes = source.ReadBytes(4);

                entry.ValueBytes = GetValueBytes(source, entry.Type, entry.Count, entry.OriginalValueBytes);

                Results.Add(entry.TagId, entry);
            }


            source.PopPosition();

            return Results;
        }

        private Dictionary<ushort, IFDEntry> CatalogueIFD(ExifDataSource source, long offset, Type tagType)
        {
            Dictionary<ushort, IFDEntry> Results = new Dictionary<ushort, IFDEntry>();

            Source.PushAndSeek(offset);

            // First 2 bytes is the number of entries in this IFD
            ushort entryCount = source.ReadUShort();

            for (ushort currentEntry = 0; currentEntry < entryCount; currentEntry++)
            {
                IFDEntry entry = tagType == null ? new IFDEntry() : new IFDEntry(tagType);

                entry.TagId = source.ReadUShort();
                entry.Type = source.ReadUShort();
                entry.Count = source.ReadUint();
                entry.OriginalValueBytes = source.ReadBytes(4);

                byte[] tmpValueBytes = new byte[entry.OriginalValueBytes.Length];
                entry.OriginalValueBytes.CopyTo(tmpValueBytes, 0);

                entry.ValueBytes = GetValueBytes(source, entry.Type, entry.Count, tmpValueBytes);

                AssignValue(entry);

                Results.Add(entry.TagId, entry);
            }

            source.PopPosition();

            return Results;
        }

        private void AssignValue(IFDEntry entry)
        {
            if (entry.Type == 2)
            {
                entry.Value = Encoding.ASCII.GetString(entry.ValueBytes).TrimEnd('\0');
                return;
            }

            if (entry.Count == 1)
            {
                switch (entry.Type)
                {
                    case 1:
                        entry.Value = entry.ValueBytes[0];
                        break;
                    case 3:
                        entry.Value = Source.ToShort(entry.ValueBytes);
                        break;
                    case 4:
                        entry.Value = Source.ToUint(entry.ValueBytes);
                        break;
                    case 5:
                        entry.Value = Source.ToURational(entry.ValueBytes);
                        break;
                }
            }
            else
            {
                switch (entry.Type)
                {
                    case 1:
                        entry.Value = ExifDataSource.GetArray<byte>(entry.ValueBytes, 1, (b) => b[0]);
                        break;
                    case 3:
                        entry.Value = ExifDataSource.GetArray<ushort>(entry.ValueBytes, GetTIFFFieldLength(entry.Type), (b) => Source.ToUShort(b));
                        break;
                    case 4:
                        entry.Value = ExifDataSource.GetArray<uint>(entry.ValueBytes, GetTIFFFieldLength(entry.Type), (b) => Source.ToUint(b));
                        break;
                    case 5:
                        entry.Value = ExifDataSource.GetArray<double>(entry.ValueBytes, GetTIFFFieldLength(entry.Type), (b) => Source.ToURational(b));
                        break;
                }
            }
        }

        private Dictionary<ushort, IFDEntry> CatalogueIFD(ExifDataSource source, long offset)
        {
            return CatalogueIFD(source, offset, null);
        }


        private byte[] GetValueBytes(ExifDataSource source, ushort type, uint count, byte[] entryValueBytes)
        {
            int dataSize = (int)(count * GetTIFFFieldLength(type));

            if (dataSize > 4)
            {
                ushort offsetAddress = source.ToUShort(entryValueBytes);


                source.PushAndSeek(offsetAddress);

                byte[] Result = source.ReadBytes(dataSize);

                source.PopPosition();


                return Result;
            }
            else
            {
                Array.Resize(ref entryValueBytes, dataSize);
                return entryValueBytes;
            }
        }


        public static byte GetTIFFFieldLength(ushort tiffDataType)
        {
            switch (tiffDataType)
            {
                case 0:
                    // Unknown datatype, therefore it can't be interpreted reliably
                    return 0;
                case 1:
                case 2:
                case 7:
                case 6:
                    return 1;
                case 3:
                case 8:
                    return 2;
                case 4:
                case 9:
                case 11:
                    return 4;
                case 5:
                case 10:
                case 12:
                    return 8;
                default:
                    throw new ApplicationException(string.Format("Unknown TIFF datatype: {0}", tiffDataType));
            }
        }


        public PhotoInfo GetPhotoInfo(TimeSpan cameraClockOffset)
        {
            return new PhotoInfo()
            {
                FileObject = ImageFile,
                DateTimeDigitized = GetPhotoDateTime(cameraClockOffset),
                PositionInfo = GetPhotoPosition()
            };
        }

        public DateTimeOffset? GetPhotoDateTime(TimeSpan cameraClockOffset)
        {
            DateTime TimeStampNoTimeZone;

            if (exifCatalog.TryGetValue((ushort)ExifTags2.DateTimeOriginal, out IFDEntry entry))
            {
                if ((entry.Value != null) && (entry.Value is String))
                {
                    if (ExifDataSource.ToDateTime((string)entry.Value, out TimeStampNoTimeZone))
                    {
                        if (makerNotesCatalog.TryGetValue((ushort)CanonMakerTags.TimeInfo, out IFDEntry TimeInfoEntry))
                        {
                            int[] TimeInfoComponents = (int[])ExifDataSource.GetArray<int>(TimeInfoEntry.ValueBytes, 4, (sourceData) => ExifDataSource.ToInt(true, sourceData));
                            return new DateTimeOffset(TimeStampNoTimeZone, new TimeSpan(0, TimeInfoComponents[1], 0)).Add(cameraClockOffset);

                        }
                        else
                        {
                            // In this case, we can't find the UTC offset, so we assume local time.  (Maybe make this an argument?)
                            return new DateTimeOffset(TimeStampNoTimeZone, TimeZoneInfo.Local.GetUtcOffset(TimeStampNoTimeZone)).Add(cameraClockOffset);
                        }
                    }
                }
            }

            return null;
        }


        public Position GetPhotoPosition()
        {
            bool hasData = false;

            // Get the position
            double? Latitude = GetPositionComponent(GPSTags.GPSLatitude);
            double? Longitude = GetPositionComponent(GPSTags.GPSLongitude);

            double? Altitude = GetDouble(GPSTags.GPSAltitude);

            string LatitudeRef = GetString(GPSTags.GPSLatitudeRef);
            string LongitudeRef = GetString(GPSTags.GPSLongitudeRef);


            Position Result = new Position();

            if ((Latitude.HasValue) && (Longitude.HasValue) && (!string.IsNullOrEmpty(LatitudeRef)) && (!string.IsNullOrEmpty(LongitudeRef)))
            {
                Result.Latitude = Convert.ToDecimal(Latitude.Value) * (LatitudeRef == "N" ? 1 : -1);
                Result.Longitude = Convert.ToDecimal(Longitude.Value) * (LongitudeRef == "E" ? 1 : -1);
                hasData = true;
            }

            if (Altitude.HasValue)
            {
                Result.Altitude = Convert.ToDecimal(Altitude.Value);
                hasData = true;
            }


            // Get the timestamp
            DateTimeOffset? GPSTimeStampUTC = GetGPSDateTime();

            if (GPSTimeStampUTC.HasValue)
            {
                Result.Time = GPSTimeStampUTC.Value;
                hasData = true;
            }

            if (hasData)
                return Result;
            else
                return null;
        }


        public double? GetDouble(GPSTags tag)
        {
            if (gpsCatalog.TryGetValue((ushort)tag, out IFDEntry entry))
            {
                if ((entry.Value != null) && (entry.Value is double))
                {
                    return Convert.ToDouble(entry.Value);
                }
            }

            return null;
        }

        public double? GetPositionComponent(GPSTags tag)
        {
            if (gpsCatalog.TryGetValue((ushort)tag, out IFDEntry entry))
            {
                if ((entry.Value != null) && (entry.Value is double[]))
                {
                    double[] components = entry.Value as double[];
                    return components[0] + (components[1] / 60.0) + (components[2] / 3600.0);
                }
            }

            return null;
        }

        public string GetString(GPSTags tag)
        {
            if (gpsCatalog.TryGetValue((ushort)tag, out IFDEntry entry))
            {
                if ((entry.Value != null) && (entry.Value is String))
                {
                    return entry.Value.ToString();
                }
            }

            return null;
        }

        public DateTimeOffset? GetGPSDateTime()
        {
            string GPSDateStamp = GetString(GPSTags.GPSDateStamp);

            if (!string.IsNullOrEmpty(GPSDateStamp))
            {
                if (DateTime.TryParseExact(GPSDateStamp, "yyyy:MM:dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime GPSDate))
                {
                    if (gpsCatalog.TryGetValue((ushort)GPSTags.GPSTimeStamp, out IFDEntry entry))
                    {
                        if ((entry.Value != null) && (entry.Value is double[]))
                        {
                            double[] components = entry.Value as double[];
                            TimeSpan timeComponent = new TimeSpan(
                                (Convert.ToInt64(components[0]) * TimeSpan.TicksPerHour) +
                                (Convert.ToInt64(components[1]) * TimeSpan.TicksPerMinute) +
                                (Convert.ToInt64(components[2]) * TimeSpan.TicksPerSecond));

                            return new DateTimeOffset(GPSDate.Add(timeComponent), new TimeSpan(0));
                        }
                    }
                }
            }

            return null;
        }


        private void DebugMessage(string format, params object[] formatValues)
        {
            Console.WriteLine(format, formatValues);
        }

        public void Dispose()
        {
            ImageFile = null;

            Source.Dispose();
            Source = null;

            ifdCatalog.Clear();
            ifdCatalog = null;

            gpsCatalog.Clear();
            gpsCatalog = null;

            exifCatalog.Clear();
            exifCatalog = null;

            makerNotesCatalog.Clear();
            makerNotesCatalog = null;
        }

        public List<PhotoInfo> GetPhotoInfoBulk(DirectoryInfo directory, TimeSpan cameraClockOffset)
        {
            throw new NotImplementedException();
        }
    }
}
