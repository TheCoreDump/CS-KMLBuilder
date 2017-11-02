using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;

namespace KmlBuilder.Kml
{
    public static class IconStyleMapBuilder
    {
        public static Style GetStyle(string idPrefix, string iconUri)
        {
            Style normalStyle = new Style()
            {
                Id = idPrefix + "_normal",
                Icon = new IconStyle()
                {
                    Scale = 1.2,
                    Color = new Color32(0, 0, 0, 255),
                    Icon = new IconStyle.IconLink(new Uri(iconUri))
                }
            };

            Style highlightedStyle = new Style()
            {
                Id = idPrefix + "_highlighted",
                Icon = new IconStyle()
                {
                    Scale = 1.4,
                    Color = new Color32(0, 0, 0, 255),
                    Icon = new IconStyle.IconLink(new Uri(iconUri))
                }
            };

            return normalStyle;

            /*

            StyleMapCollection Result = new StyleMapCollection();
            Result.Id = idPrefix + "_map";

            Result.Add(new Pair() { State = StyleState.Normal, Selector = normalStyle });
            Result.Add(new Pair() { State = StyleState.Normal, Selector = highlightedStyle });


            return Result.;
            */
        }
    }
}
