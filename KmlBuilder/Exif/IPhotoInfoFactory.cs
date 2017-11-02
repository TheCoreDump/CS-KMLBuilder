using System;
using System.Collections.Generic;
using System.IO;

namespace KmlBuilder.Exif
{
    public interface IPhotoInfoFactory
    {
        PhotoInfo GetPhotoInfo(TimeSpan cameraClockOffset);

        List<PhotoInfo> GetPhotoInfoBulk(DirectoryInfo directory, TimeSpan cameraClockOffset);
    }
}