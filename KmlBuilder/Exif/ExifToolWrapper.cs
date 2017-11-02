using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace KmlBuilder.Exif
{
    public class ExifToolWrapper : IPhotoInfoFactory
    {
        public static readonly string EXIF_TOOL_EXE = "C:\\Program Files (x86)\\ExifTool\\ExifTool.exe";
        public static readonly string EXIF_TOOL_ARGS = "-FileName -DateTimeOriginal -TimeZone -DaylightSavings -GPSDateTime -GPSLatitude -GPSLongitude -json -c \"%+f\" *.jpg";

        public PhotoInfo GetPhotoInfo(TimeSpan cameraClockOffset)
        {
            throw new NotImplementedException();
        }


        public List<PhotoInfo> GetPhotoInfoBulk(DirectoryInfo directory, TimeSpan cameraClockOffset)
        {
            StringBuilder OutputBuffer = new StringBuilder();
            StringBuilder ErrorBuffer = new StringBuilder();

            Process exifToolProcess = new Process()
            {
                StartInfo = GetExifToolStartInfo(directory)
            };

            exifToolProcess.OutputDataReceived += (sender, e) => OutputBuffer.Append(e.Data);
            exifToolProcess.ErrorDataReceived += (sender, e) => ErrorBuffer.Append(e.Data);

            exifToolProcess.Start();

            exifToolProcess.BeginOutputReadLine();
            exifToolProcess.BeginErrorReadLine();

            exifToolProcess.WaitForExit();

            exifToolProcess.CancelOutputRead();
            exifToolProcess.CancelErrorRead();

            if (exifToolProcess.ExitCode != 1)
                throw new ApplicationException(string.Format("Exit code from ExifTool.exe was not 1.  ExitCode: {0}  {1}", exifToolProcess.ExitCode, ErrorBuffer.Length > 0 ? "Error: " + ErrorBuffer.ToString() : "<<No Error Messages>>"));


            
            JArray photoData = (JArray) JToken.ReadFrom(new JsonTextReader(new StringReader(OutputBuffer.ToString())));

            List<PhotoInfo> Results = new List<PhotoInfo>();

            foreach (JObject tmpPhotoDataJObject in photoData)
            {

                Results.Add(new PhotoInfo()
                {
                    FileObject = new FileInfo(Path.Combine(directory.FullName, tmpPhotoDataJObject["FileName"].Value<string>())),

                });
            }

            return Results;
        }


        public ProcessStartInfo GetExifToolStartInfo(DirectoryInfo directory)
        {
            ProcessStartInfo Result = new ProcessStartInfo()
            {
                FileName = EXIF_TOOL_EXE,
                Arguments = EXIF_TOOL_ARGS,
                WorkingDirectory = directory.FullName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            return Result;
        }
    }
}
