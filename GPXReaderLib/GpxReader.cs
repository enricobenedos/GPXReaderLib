using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using GPXReaderLib.Models;

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
        ///  Calculate the elevation gain.
        /// </summary>
        /// <param name="treshold">Treshold in kilometers.
        /// It is needed by the algorithm to know the minimum distance between two points to calculate the elevation.
        /// A lower\higher <paramref name="treshold"/> value can cause wrong results.</param>
        /// <returns>Elevation gain in meters</returns>
        public double GetElevationGain(double treshold = 0.22)
        {
            List<TrackPoint> gpxCoordinates = GetGPXCoordinates();
            TrackPoint firstCoordinate = gpxCoordinates.First();
            TrackPoint secondCoordinate;

            double elevationGain = 0.0;
            for (int i = 0; i < gpxCoordinates.Count; i++)
            {
                if (i == gpxCoordinates.Count - 1) break;

                secondCoordinate = gpxCoordinates[i];

                var distanceKm = GetDistance(firstCoordinate.Latitude, firstCoordinate.Longitude,
                    secondCoordinate.Latitude, secondCoordinate.Longitude);
                if (distanceKm < treshold)
                {
                    continue;
                }

                var elevation = secondCoordinate.Elevation - firstCoordinate.Elevation;
                if (elevation > treshold)
                {
                    elevationGain += elevation;
                }

                firstCoordinate = secondCoordinate;
            }

            return elevationGain;
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

        /// <summary>
        /// Return total distance in kilometers
        /// </summary>
        /// <returns></returns>
        public double GetDistance()
        {
            List<XAttribute> latitudesXAtt = gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt", xmlNamespaceManager).Attributes("lat").ToList();
            List<XAttribute> longitudesXAtt = gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt", xmlNamespaceManager).Attributes("lon").ToList();

            double dist = 0.0;
            for (int i = 0; i < latitudesXAtt.Count - 1; i++)
            {
                double lat1 = double.Parse(latitudesXAtt[i].Value);
                double lat2 = double.Parse(latitudesXAtt[i + 1].Value);

                double lon1 = double.Parse(longitudesXAtt[i].Value);
                double lon2 = double.Parse(longitudesXAtt[i + 1].Value);

                // Continue if position is not chaged
                if (lat1 == lat2 && lon1 == lon2) continue;

                double rlat1 = Math.PI * lat1 / 180;
                double rlat2 = Math.PI * lat2 / 180;
                double theta = lon1 - lon2;
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

                dist += distance * 60 * 1.1515;
            }

            return dist * 1.609344;
        }

        /// <summary>
        /// Return kilometers value from gpx start to desidered date time
        /// </summary>
        /// <param name="dateTime">Lat/Lon record datetime</param>
        /// <returns>Total kilometers value</returns>
        private double GetDistance(XElement previousTrackPoint, XElement actualTrackPoint)
        {
            //"//p:gpx//p:trk//p:trkseg//p:trkpt//p:time"
            List<XAttribute> latitudesXAtt = new List<XAttribute>();
            List<XAttribute> longitudesXAtt = new List<XAttribute>();

            if (previousTrackPoint != null)
            {
                latitudesXAtt.Add(previousTrackPoint.Attribute("lat"));
                longitudesXAtt.Add(previousTrackPoint.Attribute("lon"));
            }

            latitudesXAtt.Add(actualTrackPoint.Attribute("lat"));
            longitudesXAtt.Add(actualTrackPoint.Attribute("lon"));

            double dist = 0.0;
            for (int i = 0; i < latitudesXAtt.Count - 1; i++)
            {
                double lat1 = double.Parse(latitudesXAtt[i].Value);
                double lat2 = double.Parse(latitudesXAtt[i + 1].Value);

                double lon1 = double.Parse(longitudesXAtt[i].Value);
                double lon2 = double.Parse(longitudesXAtt[i + 1].Value);

                //Continue if position is not chaged
                if (lat1 == lat2 && lon1 == lon2) continue;

                double rlat1 = Math.PI * lat1 / 180;
                double rlat2 = Math.PI * lat2 / 180;
                double theta = lon1 - lon2;
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

                dist += distance * 60 * 1.1515;
            }

            return dist * 1.609344;
        }

        // TODO: GetDistance methods need to be cleaned up due to code reuse

        /// <summary>
        /// Calulcate distance between two coordinates (latitude, logitude).
        /// </summary>
        /// <returns>Kilometers distance</returns>
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

        /// <summary>
        /// Return a list of latitude and longitude coordinates
        /// </summary>
        /// <returns></returns>
        public List<TrackPoint> GetGPXCoordinates()
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

        /// <summary>
        /// Return a list of elevation to obtain a detailed altimetry
        /// </summary>
        /// <returns></returns>
        public GpxAltimetry GetGpxAltimetry()
        {
            double minElevation = GetElevation(ElevationType.Min);
            double maxElevation = GetElevation(ElevationType.Max);
            double avgElevation = GetElevation(ElevationType.Avg);

            List<Altimetry> altimetries = new List<Altimetry>();
            XElement previousTrackPoint = null;

            //Get list of all registered info record
            foreach (XElement trackPoint in gpx.XPathSelectElements("/p:gpx/p:trk/p:trkseg/p:trkpt", xmlNamespaceManager))
            {
                //Get current elevation if available else continue with next cycle˙
                bool convResult = double.TryParse(trackPoint.XPathSelectElement("p:ele", xmlNamespaceManager)?.Value, out double actualElevation);
                if (!convResult)
                {
                    continue;
                }

                //Get distance between previous trackPoint and the current one
                double distance = GetDistance(previousTrackPoint, trackPoint);
                //Get last inserted altimetry distance
                double lastDistanceValue = altimetries.LastOrDefault() == null ? 0.0 : altimetries.Last().Kilometers;

                //Obtain the actual distance value from start sum actual value with the last one
                double actualDistance = distance + lastDistanceValue;

                altimetries.Add(new Altimetry(actualElevation, actualDistance));

                previousTrackPoint = trackPoint;
            }

            return new GpxAltimetry(minElevation, maxElevation, avgElevation, altimetries);
        }
    }
}

