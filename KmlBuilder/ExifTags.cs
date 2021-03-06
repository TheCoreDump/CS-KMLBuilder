﻿using System;

namespace KmlBuilder
{
    public enum ExifTags2 : int
    {
        InteropIndex = 0x0001,
        InteropVersion = 0x0002,
        ProcessingSoftware = 0x000b,
        SubfileType = 0x00fe,
        OldSubfileType = 0x00ff,
        ImageWidth = 0x0100,
        ImageHeight = 0x0101,
        BitsPerSample = 0x0102,
        Compression = 0x0103,
        PhotometricInterpretation = 0x0106,
        Thresholding = 0x0107,
        CellWidth = 0x0108,
        CellLength = 0x0109,
        FillOrder = 0x010a,
        DocumentName = 0x010d,
        ImageDescription = 0x010e,
        Make = 0x010f,
        Model = 0x0110,
        StripOffsets = 0x0111,
        Orientation = 0x0112,
        SamplesPerPixel = 0x0115,
        RowsPerStrip = 0x0116,
        StripByteCounts = 0x0117,
        MinSampleValue = 0x0118,
        MaxSampleValue = 0x0119,
        XResolution = 0x011a,
        YResolution = 0x011b,
        PlanarConfiguration = 0x011c,
        PageName = 0x011d,
        XPosition = 0x011e,
        YPosition = 0x011f,
        FreeOffsets = 0x0120,
        FreeByteCounts = 0x0121,
        GrayResponseUnit = 0x0122,
        GrayResponseCurve = 0x0123,
        T4Options = 0x0124,
        T6Options = 0x0125,
        ResolutionUnit = 0x0128,
        PageNumber = 0x0129,
        ColorResponseUnit = 0x012c,
        TransferFunction = 0x012d,
        Software = 0x0131,
        ModifyDate = 0x0132,
        Artist = 0x013b,
        HostComputer = 0x013c,
        Predictor = 0x013d,
        WhitePoint = 0x013e,
        PrimaryChromaticities = 0x013f,
        ColorMap = 0x0140,
        HalftoneHints = 0x0141,
        TileWidth = 0x0142,
        TileLength = 0x0143,
        TileOffsets = 0x0144,
        TileByteCounts = 0x0145,
        BadFaxLines = 0x0146,
        CleanFaxData = 0x0147,
        ConsecutiveBadFaxLines = 0x0148,
        SubIFD = 0x014a,
        InkSet = 0x014c,
        InkNames = 0x014d,
        NumberofInks = 0x014e,
        DotRange = 0x0150,
        TargetPrinter = 0x0151,
        ExtraSamples = 0x0152,
        SampleFormat = 0x0153,
        SMinSampleValue = 0x0154,
        SMaxSampleValue = 0x0155,
        TransferRange = 0x0156,
        ClipPath = 0x0157,
        XClipPathUnits = 0x0158,
        YClipPathUnits = 0x0159,
        Indexed = 0x015a,
        JPEGTables = 0x015b,
        OPIProxy = 0x015f,
        GlobalParametersIFD = 0x0190,
        ProfileType = 0x0191,
        FaxProfile = 0x0192,
        CodingMethods = 0x0193,
        VersionYear = 0x0194,
        ModeNumber = 0x0195,
        Decode = 0x01b1,
        DefaultImageColor = 0x01b2,
        T82Options = 0x01b3,
        JPEGTables2 = 0x01b5,
        JPEGProc = 0x0200,
        ThumbnailOffset = 0x0201,
        ThumbnailLength = 0x0202,
        JPEGRestartInterval = 0x0203,
        JPEGLosslessPredictors = 0x0205,
        JPEGPointTransforms = 0x0206,
        JPEGQTables = 0x0207,
        JPEGDCTables = 0x0208,
        JPEGACTables = 0x0209,
        YCbCrCoefficients = 0x0211,
        YCbCrSubSampling = 0x0212,
        YCbCrPositioning = 0x0213,
        ReferenceBlackWhite = 0x0214,
        StripRowCounts = 0x022f,
        ApplicationNotes = 0x02bc,
        USPTOMiscellaneous = 0x03e7,
        RelatedImageFileFormat = 0x1000,
        RelatedImageWidth = 0x1001,
        RelatedImageHeight = 0x1002,
        Rating = 0x4746,
        XP_DIP_XML = 0x4747,
        StitchInfo = 0x4748,
        RatingPercent = 0x4749,
        SonyRawFileType = 0x7000,
        VignettingCorrParams = 0x7032,
        ChromaticAberrationCorrParams = 0x7035,
        DistortionCorrParams = 0x7037,
        ImageID = 0x800d,
        WangTag1 = 0x80a3,
        WangAnnotation = 0x80a4,
        WangTag3 = 0x80a5,
        WangTag4 = 0x80a6,
        ImageReferencePoints = 0x80b9,
        RegionXformTackPoint = 0x80ba,
        WarpQuadrilateral = 0x80bb,
        AffineTransformMat = 0x80bc,
        Matteing = 0x80e3,
        DataType = 0x80e4,
        ImageDepth = 0x80e5,
        TileDepth = 0x80e6,
        ImageFullWidth = 0x8214,
        ImageFullHeight = 0x8215,
        TextureFormat = 0x8216,
        WrapModes = 0x8217,
        FovCot = 0x8218,
        MatrixWorldToScreen = 0x8219,
        MatrixWorldToCamera = 0x821a,
        Model2 = 0x827d,
        CFARepeatPatternDim = 0x828d,
        CFAPattern2 = 0x828e,
        BatteryLevel = 0x828f,
        KodakIFD = 0x8290,
        Copyright = 0x8298,
        ExposureTime = 0x829a,
        FNumber = 0x829d,
        MDFileTag = 0x82a5,
        MDScalePixel = 0x82a6,
        MDColorTable = 0x82a7,
        MDLabName = 0x82a8,
        MDSampleInfo = 0x82a9,
        MDPrepDate = 0x82aa,
        MDPrepTime = 0x82ab,
        MDFileUnits = 0x82ac,
        PixelScale = 0x830e,
        AdventScale = 0x8335,
        AdventRevision = 0x8336,
        UIC1Tag = 0x835c,
        UIC2Tag = 0x835d,
        UIC3Tag = 0x835e,
        UIC4Tag = 0x835f,
        IPTCNAA = 0x83bb,
        IntergraphPacketData = 0x847e,
        IntergraphFlagRegisters = 0x847f,
        IntergraphMatrix = 0x8480,
        INGRReserved = 0x8481,
        ModelTiePoint = 0x8482,
        Site = 0x84e0,
        ColorSequence = 0x84e1,
        IT8Header = 0x84e2,
        RasterPadding = 0x84e3,
        BitsPerRunLength = 0x84e4,
        BitsPerExtendedRunLength = 0x84e5,
        ColorTable = 0x84e6,
        ImageColorIndicator = 0x84e7,
        BackgroundColorIndicator = 0x84e8,
        ImageColorValue = 0x84e9,
        BackgroundColorValue = 0x84ea,
        PixelIntensityRange = 0x84eb,
        TransparencyIndicator = 0x84ec,
        ColorCharacterization = 0x84ed,
        HCUsage = 0x84ee,
        TrapIndicator = 0x84ef,
        CMYKEquivalent = 0x84f0,
        SEMInfo = 0x8546,
        AFCP_IPTC = 0x8568,
        PixelMagicJBIGOptions = 0x85b8,
        JPLCartoIFD = 0x85d7,
        ModelTransform = 0x85d8,
        WB_GRGBLevels = 0x8602,
        LeafData = 0x8606,
        PhotoshopSettings = 0x8649,
        ExifOffset = 0x8769,
        ICC_Profile = 0x8773,
        TIFF_FXExtensions = 0x877f,
        MultiProfiles = 0x8780,
        SharedData = 0x8781,
        T88Options = 0x8782,
        ImageLayer = 0x87ac,
        GeoTiffDirectory = 0x87af,
        GeoTiffDoubleParams = 0x87b0,
        GeoTiffAsciiParams = 0x87b1,
        JBIGOptions = 0x87be,
        ExposureProgram = 0x8822,
        SpectralSensitivity = 0x8824,
        GPSInfo = 0x8825,
        ISO = 0x8827,
        OptoElectricConvFactor = 0x8828,
        Interlace = 0x8829,
        TimeZoneOffset = 0x882a,
        SelfTimerMode = 0x882b,
        SensitivityType = 0x8830,
        StandardOutputSensitivity = 0x8831,
        RecommendedExposureIndex = 0x8832,
        ISOSpeed = 0x8833,
        ISOSpeedLatitudeyyy = 0x8834,
        ISOSpeedLatitudezzz = 0x8835,
        FaxRecvParams = 0x885c,
        FaxSubAddress = 0x885d,
        FaxRecvTime = 0x885e,
        FedexEDR = 0x8871,
        LeafSubIFD = 0x888a,
        ExifVersion = 0x9000,
        DateTimeOriginal = 0x9003,
        CreateDate = 0x9004,
        GooglePlusUploadCode = 0x9009,
        OffsetTime = 0x9010,
        OffsetTimeOriginal = 0x9011,
        OffsetTimeDigitized = 0x9012,
        ComponentsConfiguration = 0x9101,
        CompressedBitsPerPixel = 0x9102,
        ShutterSpeedValue = 0x9201,
        ApertureValue = 0x9202,
        BrightnessValue = 0x9203,
        ExposureCompensation = 0x9204,
        MaxApertureValue = 0x9205,
        SubjectDistance = 0x9206,
        MeteringMode = 0x9207,
        LightSource = 0x9208,
        Flash = 0x9209,
        FocalLength = 0x920a,
        FlashEnergy = 0x920b,
        SpatialFrequencyResponse = 0x920c,
        Noise = 0x920d,
        FocalPlaneXResolution = 0x920e,
        FocalPlaneYResolution = 0x920f,
        FocalPlaneResolutionUnit = 0x9210,
        ImageNumber = 0x9211,
        SecurityClassification = 0x9212,
        ImageHistory = 0x9213,
        SubjectArea = 0x9214,
        ExposureIndex = 0x9215,
        TIFFEPStandardID = 0x9216,
        SensingMethod = 0x9217,
        CIP3DataFile = 0x923a,
        CIP3Sheet = 0x923b,
        CIP3Side = 0x923c,
        StoNits = 0x923f,
        MakerNotes = 0x927c,
        UserComment = 0x9286,
        SubSecTime = 0x9290,
        SubSecTimeOriginal = 0x9291,
        SubSecTimeDigitized = 0x9292,
        MSDocumentText = 0x932f,
        MSPropertySetStorage = 0x9330,
        MSDocumentTextPosition = 0x9331,
        ImageSourceData = 0x935c,
        AmbientTemperature = 0x9400,
        Humidity = 0x9401,
        Pressure = 0x9402,
        WaterDepth = 0x9403,
        Acceleration = 0x9404,
        CameraElevationAngle = 0x9405,
        XPTitle = 0x9c9b,
        XPComment = 0x9c9c,
        XPAuthor = 0x9c9d,
        XPKeywords = 0x9c9e,
        XPSubject = 0x9c9f,
        FlashpixVersion = 0xa000,
        ColorSpace = 0xa001,
        ExifImageWidth = 0xa002,
        ExifImageHeight = 0xa003,
        RelatedSoundFile = 0xa004,
        InteropOffset = 0xa005,
        SamsungRawPointersOffset = 0xa010,
        SamsungRawPointersLength = 0xa011,
        SamsungRawByteOrder = 0xa101,
        SamsungRawUnknown = 0xa102,
        FlashEnergy2 = 0xa20b,
        SpatialFrequencyResponse2 = 0xa20c,
        Noise2 = 0xa20d,
        FocalPlaneXResolution2 = 0xa20e,
        FocalPlaneYResolution2 = 0xa20f,
        FocalPlaneResolutionUnit2 = 0xa210,
        ImageNumber2 = 0xa211,
        SecurityClassification2 = 0xa212,
        ImageHistory2 = 0xa213,
        SubjectLocation = 0xa214,
        ExposureIndex2 = 0xa215,
        TIFFEPStandardID2 = 0xa216,
        SensingMethod2 = 0xa217,
        FileSource = 0xa300,
        SceneType = 0xa301,
        CFAPattern = 0xa302,
        CustomRendered = 0xa401,
        ExposureMode = 0xa402,
        WhiteBalance = 0xa403,
        DigitalZoomRatio = 0xa404,
        FocalLengthIn35mmFormat = 0xa405,
        SceneCaptureType = 0xa406,
        GainControl = 0xa407,
        Contrast = 0xa408,
        Saturation = 0xa409,
        Sharpness = 0xa40a,
        DeviceSettingDescription = 0xa40b,
        SubjectDistanceRange = 0xa40c,
        ImageUniqueID = 0xa420,
        OwnerName = 0xa430,
        SerialNumber = 0xa431,
        LensInfo = 0xa432,
        LensMake = 0xa433,
        LensModel = 0xa434,
        LensSerialNumber = 0xa435,
        GDALMetadata = 0xa480,
        GDALNoData = 0xa481,
        Gamma = 0xa500,
        ExpandSoftware = 0xafc0,
        ExpandLens = 0xafc1,
        ExpandFilm = 0xafc2,
        ExpandFilterLens = 0xafc3,
        ExpandScanner = 0xafc4,
        ExpandFlashLamp = 0xafc5,
        PixelFormat = 0xbc01,
    }
}
