using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using GPXReaderLib.Interfaces;
using GPXReaderLib.Models;

namespace GPXReaderLib
{
    public class GpxReader : IGpxReader
    {
        private readonly XDocument gpx;
        private readonly XmlNamespaceManager xmlNamespaceManager;

        public GpxReader(XDocument gpx, XmlNamespaceManager xmlNamespaceManager)
        {
            this.gpx = gpx;
            this.xmlNamespaceManager = xmlNamespaceManager;
        }

        public string GetGpxName()
        {
            return gpx.XPathSelectElement("//p:gpx//p:trk//p:name", xmlNamespaceManager).Value;
        }

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

        public double GetElevationGain(double treshold = 0.22)
        {
            IEnumerable<TrackPoint> trackPoints = GetGpxCoordinates();
            TrackPoint trackPoint1 = trackPoints.First();
            TrackPoint trackPoint2 = trackPoints.First();

            double elevationGain = 0.0;
            foreach (var trackPoint in trackPoints)
            {
                trackPoint2 = trackPoint;

                var distanceKm = GetDistance(trackPoint1.Latitude, trackPoint1.Longitude,
                    trackPoint2.Latitude, trackPoint2.Longitude);
                if (distanceKm < treshold)
                {
                    continue;
                }

                var elevation = trackPoint2.Elevation - trackPoint1.Elevation;
                if (elevation > treshold)
                {
                    elevationGain += elevation;
                }

                trackPoint1 = trackPoint2;
            }

            return elevationGain;
        }

        public DateTime GetStartDt()
        {
            return gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt//p:time", xmlNamespaceManager).Min(x => DateTime.Parse(x.Value));
        }

        public DateTime GetEndDt()
        {
            return gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt//p:time", xmlNamespaceManager).Max(x => DateTime.Parse(x.Value));
        }

        public TimeSpan GetDuration()
        {
            DateTime startDt = GetStartDt();
            DateTime endDt = GetEndDt();

            return endDt - startDt;
        }

        public double GetDistance()
        {
            IEnumerable<TrackPoint> trackPoints = GetGpxCoordinates();
            TrackPoint trackPoint1 = trackPoints.First();
            TrackPoint trackPoint2 = trackPoints.First();

            double distance = 0.0;
            foreach (var trackPoint in trackPoints)
            {
                trackPoint1 = trackPoint2;
                trackPoint2 = trackPoint;

                distance += GetDistance(trackPoint1.Latitude, trackPoint1.Longitude,
                    trackPoint2.Latitude, trackPoint2.Longitude);

            }

            return distance;
        }

        public IEnumerable<TrackPoint> GetGpxCoordinates()
        {
            List<XAttribute> latitudesXAtt = gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt", xmlNamespaceManager).Attributes("lat").ToList();
            List<XAttribute> longitudesXAtt = gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt", xmlNamespaceManager).Attributes("lon").ToList();
            List<XElement> elevationsXEl = gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt//p:ele", xmlNamespaceManager).ToList();

            List<TrackPoint> gPXCoordinates = new List<TrackPoint>();
            for (int i = 0; i < latitudesXAtt.Count; i++) //assume that two lists have same XAttributes count
            {
                double latitude = double.Parse(latitudesXAtt[i].Value);
                double longitude = double.Parse(longitudesXAtt[i].Value);
                double elevation = double.Parse(elevationsXEl[i].Value);

                gPXCoordinates.Add(new TrackPoint(latitude, longitude, elevation));
            }

            return gPXCoordinates;
        }

        public GpxAltimetry GetGpxAltimetry()
        {
            double minElevation = GetElevation(ElevationType.Min);
            double maxElevation = GetElevation(ElevationType.Max);
            double avgElevation = GetElevation(ElevationType.Avg);

            List<Altimetry> altimetries = new List<Altimetry>();
            TrackPoint previousTrackPoint = null;

            //Get list of all registered info record
            foreach (TrackPoint trackPoint in GetGpxCoordinates())
            {
                if(previousTrackPoint == null)
                {
                    previousTrackPoint = trackPoint;
                }

                //Get distance between previous trackPoint and the current one
                double distance = GetDistance(previousTrackPoint.Latitude, previousTrackPoint.Longitude,
                    trackPoint.Latitude, trackPoint.Longitude);
                //Get last inserted altimetry distance
                double lastDistanceValue = altimetries.LastOrDefault() == null ? 0.0 : altimetries.Last().Kilometers;

                //Obtain the actual distance value from start sum actual value with the last one
                double actualDistance = distance + lastDistanceValue;

                altimetries.Add(new Altimetry(trackPoint.Elevation, actualDistance));

                previousTrackPoint = trackPoint;
            }

            return new GpxAltimetry(minElevation, maxElevation, avgElevation, altimetries);
        }

        private double GetDistance(double latitude, double longitude, double latitude2, double longitude2)
        {
            // Continue if position is not chaged
            if (latitude == latitude2 && longitude == longitude2) return 0;

            double rlat1 = Math.PI * latitude / 180;
            double rlat2 = Math.PI * latitude2 / 180;
            double theta = longitude - longitude2;
            double rtheta = Math.PI * theta / 180;
            double distance =
               Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
               Math.Cos(rlat2) * Math.Cos(rtheta);

            if (distance > 1)
            {
                distance = 1;
            }
            else if (distance < -1)
            {
                distance = -1;
            }

            distance = Math.Acos(distance);
            distance = distance * 180 / Math.PI;

            return distance * 60 * 1.1515 * 1.609344;
        }
    }
}

