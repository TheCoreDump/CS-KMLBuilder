using System;

namespace KmlBuilder
{
    public enum GPSTags : int
    {
        GPSVersionID = 0x0000,
        GPSLatitudeRef = 0x0001,
        GPSLatitude = 0x0002,
        GPSLongitudeRef = 0x0003,
        GPSLongitude = 0x0004,
        GPSAltitudeRef = 0x0005,
        GPSAltitude = 0x0006,
        GPSTimeStamp = 0x0007,
        GPSSatellites = 0x0008,
        GPSStatus = 0x0009,
        GPSMeasureMode = 0x000a,
        GPSDOP = 0x000b,
        GPSSpeedRef = 0x000c,
        GPSSpeed = 0x000d,
        GPSTrackRef = 0x000e,
        GPSTrack = 0x000f,
        GPSImgDirectionRef = 0x0010,
        GPSImgDirection = 0x0011,
        GPSMapDatum = 0x0012,
        GPSDestLatitudeRef = 0x0013,
        GPSDestLatitude = 0x0014,
        GPSDestLongitudeRef = 0x0015,
        GPSDestLongitude = 0x0016,
        GPSDestBearingRef = 0x0017,
        GPSDestBearing = 0x0018,
        GPSDestDistanceRef = 0x0019,
        GPSDestDistance = 0x001a,
        GPSProcessingMethod = 0x001b,
        GPSAreaInformation = 0x001c,
        GPSDateStamp = 0x001d,
        GPSDifferential = 0x001e,
        GPSHPositioningError = 0x001f,
    }
}
