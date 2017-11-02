using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace KmlBuilder.Exif
{
    public class ExifDataSource : IDisposable
    {
        private static readonly Regex NullDateTimeMatcher = new Regex(@"^[\s0]{4}[:\s][\s0]{2}[:\s][\s0]{5}[:\s][\s0]{2}[:\s][\s0]{2}$");


        public ExifDataSource()
        {
            PositionStack = new Stack<long>();
            LittleEndianStack = new Stack<bool>();

            LittleEndianStack.Push(false);
        }


        private Stream ImageStream { get; set; }

        private BinaryReader ImageStreamReader { get; set; }

        private long TiffHeaderStart { get; set; }

        private ushort ExifHeaderLength { get; set; }

        public uint IFDOffset { get; set; }

        protected Stack<long> PositionStack { get; set; }



        private Stack<bool> LittleEndianStack { get; set; }

        public bool IsLittleEndian
        {
            get
            {
                return LittleEndianStack.Peek();
            }
        }

        public void PushLittleEndian(bool isLittleEndian)
        {
            LittleEndianStack.Push(isLittleEndian);
        }


        public bool PopLittleEndian()
        {
            return LittleEndianStack.Pop();
        }

        public void Initialize(Stream sourceStream)
        {
            // Move the relevant data into a memory stream.
            ImageStream = BuildMemoryStream(sourceStream);

            // Wrap the memory stream in a binary reader.
            ImageStreamReader = new BinaryReader(ImageStream);


            // Check that the Exif header is the first piece of data.
            if (ReadString(4) != "Exif")
                throw new ApplicationException("Exif data not found");

            // 2 zero bytes
            if (ReadUShort() != 0)
                throw new ApplicationException("Malformed Exif data");


            // Set the TiffHeaderOffset
            TiffHeaderStart = ImageStream.Position;


            // What byte align will be used for the TIFF part of the document? II for Intel, MM for Motorola
            LittleEndianStack.Push(ReadString(2) == "II");

            // Next 2 bytes are always the same.
            if (ReadUShort() != 0x002A)
                throw new ApplicationException("Error in TIFF data");

            // Get the offset to the IFD (image file directory)
            IFDOffset = ReadUint();
        }


        private Stream BuildMemoryStream(Stream sourceStream)
        {
            BinaryReader SourceFileReader = new BinaryReader(sourceStream);

            if (ToUShort(ReadBytes(SourceFileReader, 2)) != 0xFFD8)
                throw new ApplicationException("File is not a valid JPEG");


            // Get the next tag.
            byte markerStart;
            byte markerNumber = 0;

            while (((markerStart = SourceFileReader.ReadByte()) == 0xFF) && (markerNumber = SourceFileReader.ReadByte()) != 0xE1)
            {
                // Get the length of the data.
                ushort dataLength = ToUShort(ReadBytes(SourceFileReader, 2));

                // Jump to the end of the data (note that the size field includes its own size)!
                int offset = dataLength - 2;
                long expectedPosition = sourceStream.Position + offset;
                sourceStream.Seek(offset, SeekOrigin.Current);

                // It's unfortunate that we have to do this, but some streams report CanSeek but don't actually seek
                // (i.e. Microsoft.Phone.Tasks.DssPhotoStream), so we have to make sure the seek actually worked. The check is performed
                // here because this is the first time we perform a seek operation.
                if (sourceStream.Position != expectedPosition)
                    throw new ApplicationException(string.Format("Supplied stream of type {0} reports CanSeek=true, but fails to seek", sourceStream.GetType()));
            }


            // It's only success if we found the 0xFFE1 marker
            if (markerStart != 0xFF || markerNumber != 0xE1)
                throw new ApplicationException("Could not find Exif data block");


            // The next 2 bytes are the size of the Exif data.
            ExifHeaderLength = ToUShort(ReadBytes(SourceFileReader, 2));


            // Copy the exif data to the memory stream
            MemoryStream tmpMemoryStream = new MemoryStream(ExifHeaderLength);
            sourceStream.CopyTo(tmpMemoryStream, 4096);

            // Reset the stream to the beginning.
            tmpMemoryStream.Seek(0, SeekOrigin.Begin);

            return tmpMemoryStream;
        }


        private void DebugMessage(string format, params object[] formatValues)
        {
            Console.WriteLine(format, formatValues);
        }


        public static bool ToDateTime(string str, out DateTime result)
        {
            // From page 28 of the Exif 2.2 spec (http://www.exif.org/Exif2-2.PDF): 

            // "When the field is left blank, it is treated as unknown ... When the date and time are unknown, 
            // all the character spaces except colons (":") may be filled with blank characters"
            if (string.IsNullOrEmpty(str) || NullDateTimeMatcher.IsMatch(str))
            {
                result = DateTime.MinValue;
                return false;
            }

            // There are 2 types of date - full date/time stamps, and plain dates. Dates are 10 characters long.
            if (str.Length == 10)
            {
                result = DateTime.ParseExact(str, "yyyy:MM:dd", CultureInfo.InvariantCulture);
                return true;
            }

            // "The format is "YYYY:MM:DD HH:MM:SS" with time shown in 24-hour format, and the date and time separated by one blank character [20.H].
            result = DateTime.ParseExact(str, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
            return true;
        }


        public static Array GetArray<T>(byte[] data, int elementLengthBytes, Func<byte[], T> converter)
        {
            Array convertedData = new T[data.Length / elementLengthBytes];

            var buffer = new byte[elementLengthBytes];

            // Read each element from the array
            for (int elementCount = 0; elementCount < data.Length / elementLengthBytes; elementCount++)
            {
                // Place the data for the current element into the buffer
                Array.Copy(data, elementCount * elementLengthBytes, buffer, 0, elementLengthBytes);

                // Process the data and place it into the output array
                convertedData.SetValue(converter(buffer), elementCount);
            }

            return convertedData;
        }


        public double ToRational(byte[] data)
        {
            return ToRational(IsLittleEndian, data);
        }


        public static double ToRational(bool isLittleEndian, byte[] data)
        {
            var fraction = ToRationalFraction(isLittleEndian, data);

            return fraction[0] / (double)fraction[1];
        }


        public static uint[] ToURationalFraction(bool isLittleEndian, byte[] data)
        {
            var numeratorData = new byte[4];
            var denominatorData = new byte[4];

            Array.Copy(data, numeratorData, 4);
            Array.Copy(data, 4, denominatorData, 0, 4);

            uint numerator = ExifDataSource.ToUint(isLittleEndian, numeratorData);
            uint denominator = ExifDataSource.ToUint(isLittleEndian, denominatorData);

            return new[] { numerator, denominator };
        }


        public double ToURational(byte[] data)
        {
            return ToURational(IsLittleEndian, data);
        }


        public static double ToURational(bool isLittleEndian, byte[] data)
        {
            var fraction = ToURationalFraction(isLittleEndian, data);

            return fraction[0] / (double)fraction[1];
        }


        public static int[] ToRationalFraction(bool isLittleEndian, byte[] data)
        {
            var numeratorData = new byte[4];
            var denominatorData = new byte[4];

            Array.Copy(data, numeratorData, 4);
            Array.Copy(data, 4, denominatorData, 0, 4);

            int numerator = ExifDataSource.ToInt(isLittleEndian, numeratorData);
            int denominator = ExifDataSource.ToInt(isLittleEndian, denominatorData);

            return new[] { numerator, denominator };
        }


        public ushort ReadUShort()
        {
            return ToUShort(ReadBytes(2));
        }


        public static ushort ToUShort(bool isLittleEndian, byte[] data)
        {
            if (isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToUInt16(data, 0);
        }


        public ushort ToUShort(byte[] data)
        {
            return ToUShort(IsLittleEndian, data);
        }

        protected string ReadString(BinaryReader reader, int chars)
        {
            var bytes = ReadBytes(reader, chars);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length).TrimEnd('\0');
        }


        public string ReadString(int chars)
        {
            var bytes = ReadBytes(chars);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length).TrimEnd('\0');
        }


        private static byte[] ReadBytes(BinaryReader reader, int byteCount)
        {
            var bytes = reader.ReadBytes(byteCount);

            // ReadBytes may return less than the bytes requested if the end of the stream is reached
            if (bytes.Length != byteCount)
                throw new EndOfStreamException();

            return bytes;
        }

        public byte[] ReadBytes(int byteCount)
        {
            return ReadBytes(ImageStreamReader, byteCount);
        }


        public uint ReadUint()
        {
            return ToUint(ReadBytes(4));
        }

        public static uint ToUint(bool isLittleEndian, byte[] data)
        {
            if (isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToUInt32(data, 0);
        }

        public uint ToUint(byte[] data)
        {
            return ToUint(IsLittleEndian, data);
        }

        public static int ToInt(bool isLittleEndian, byte[] data)
        {
            if (isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt32(data, 0);
        }

        public int ToInt(byte[] data)
        {
            return ToInt(IsLittleEndian, data);
        }

        public static double ToDouble(bool isLittleEndian, byte[] data)
        {
            if (isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToDouble(data, 0);
        }

        public double ToDouble(byte[] data)
        {
            return ToDouble(IsLittleEndian, data);
        }

        public static float ToSingle(bool isLittleEndian, byte[] data)
        {
            if (isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToSingle(data, 0);
        }


        public static short ToShort(bool isLittleEndian, byte[] data)
        {
            if (isLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt16(data, 0);
        }


        public short ToShort(byte[] data)
        {
            return ToShort(IsLittleEndian, data);
        }

        public sbyte ToSByte(byte[] data)
        {
            // An sbyte should just be a byte with an offset range.
            return (sbyte)(data[0] - byte.MaxValue);
        }


        public void PushAndSeek(long offset)
        {
            PushPosition();
            SeekOffset(offset);
        }

        public void PushPosition()
        {
            PositionStack.Push(ImageStream.Position - TiffHeaderStart);
        }


        public void PopPosition()
        {
            SeekOffset(PositionStack.Pop());
        }

        public void SeekOffset(long offset)
        {
            ImageStream.Seek(TiffHeaderStart + offset, SeekOrigin.Begin);
        }

        public long CurrentOffset()
        {
            return ImageStream.Position - TiffHeaderStart;
        }

        public void Dispose()
        {
            ImageStream.Dispose();
            ImageStream = null;

            ImageStreamReader.Dispose();
            ImageStreamReader = null;

            PositionStack.Clear();
            PositionStack = null;
        }
    }
}
