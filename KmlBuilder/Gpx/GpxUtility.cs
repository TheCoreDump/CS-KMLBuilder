using System;

namespace KmlBuilder.Gpx
{
    public static class GpxUtility
    {
        public static DateTime GetGPSDateTime(DateTime dateData, double[] data)
        {
            if (data != null)
            {
                System.TimeSpan TimePortion = new System.TimeSpan(Convert.ToInt32(data[0]), Convert.ToInt32(data[1]), Convert.ToInt32(data[2]));
                return dateData.Date.Add(TimePortion);
            }

            return new DateTime(2000, 1, 1);
        }
    }
}
