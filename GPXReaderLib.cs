using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GPXReaderLib
{
    public class GPXReader
    {
        private readonly XDocument gpx;
        private readonly XmlNamespaceManager xmlNamespaceManager;

        public enum ElevationType { Min, Max, Avg }

        public GPXReader(XDocument gpx, XmlNamespaceManager xmlNamespaceManager)
        {
            this.gpx = gpx;
            this.xmlNamespaceManager = xmlNamespaceManager;
        }

        /// <summary>
        /// Return GPX name
        /// </summary>
        /// <returns></returns>
        public string GetGPXName()
        {
            return gpx.XPathSelectElement("//p:gpx//p:trk//p:name", xmlNamespaceManager).Value;
        }

        /// <summary>
        /// Return GPX elevation based on type
        /// </summary>
        /// <param name="elevationType"></param>
        /// <returns></returns>
        public double GetElevation(ElevationType elevationType)
        {
            switch (elevationType)
            {
                case ElevationType.Min:
                    return gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt//p:ele", xmlNamespaceManager).Min(x => double.Parse(x.Value));
                case ElevationType.Max:
                    return gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt//p:ele", xmlNamespaceManager).Max(x => double.Parse(x.Value));
                case ElevationType.Avg:
                    return gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt//p:ele", xmlNamespaceManager).Average(x => double.Parse(x.Value));
                default:
                    return 0.0;
            }
        }

        /// <summary>
        /// Return GPX start datetime
        /// </summary>
        /// <returns></returns>
        public DateTime GetStartDt()
        {
            return gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt//p:time", xmlNamespaceManager).Min(x => DateTime.Parse(x.Value));
        }

        /// <summary>
        /// Return GPX end datetime
        /// </summary>
        /// <returns></returns>
        public DateTime GetEndDt()
        {
            return gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt//p:time", xmlNamespaceManager).Max(x => DateTime.Parse(x.Value));
        }

        /// <summary>
        /// Return GPX duration
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetDuration()
        {
            DateTime startDt = GetStartDt();
            DateTime endDt = GetEndDt();

            return endDt - startDt;
        }
    }
}
