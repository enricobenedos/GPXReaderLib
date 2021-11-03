using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using StravaGPXReaderLib.Models;

namespace StravaGPXReaderLib
{
    public class GPXReader
    {
        private readonly XDocument gpx;
        private readonly XmlNamespaceManager xmlNamespaceManager;

        public enum ElevationType { Min, Max, Avg }

        // Enum values generated courtesy of this post: https://stackoverflow.com/questions/61903437/what-do-the-strava-gpx-type-type-elements-mean
        public enum ActivityType
        {
            Ride,
            Alpine_Ski,
            Backcountry_Ski,
            Hike,
            Ice_Skate,
            Inline_Skate,
            Nordic_Ski,
            Roller_Ski,
            Run,
            Walk,
            Workout,
            Snowboard,
            Snowshoe,
            Kitesurf,
            Windsurf,
            Swim,
            Virtual_Ride,
            EBike_Ride,
            Velomobile,
            Canoe,
            Kayaking,
            Rowing,
            Stand_Up_Paddling,
            Surfing,
            Crossfit,
            Elliptical,
            Rock_Climb,
            StairStepper,
            Weight_Training,
            Yoga,
            Handcycle,
            Wheelchair,
            Virtual_Run,
            None
        }

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
        /// Return Activity Type based on integer value retrieved
        /// </summary>
        /// <param name="elevationType"></param>
        /// <returns></returns>
        public ActivityType GetActivityType()
        {
            var rawType = ((int)gpx.XPathSelectElement("//p:gpx//p:trk//p:type", xmlNamespaceManager));

            switch (rawType)
            {
                case 1: return ActivityType.Ride;
                case 2: return ActivityType.Alpine_Ski;
                case 3: return ActivityType.Backcountry_Ski;
                case 4: return ActivityType.Hike;
                case 5: return ActivityType.Ice_Skate;
                case 6: return ActivityType.Inline_Skate;
                case 7: return ActivityType.Nordic_Ski;
                case 8: return ActivityType.Roller_Ski;
                case 9: return ActivityType.Run;
                case 10: return ActivityType.Walk;
                case 11: return ActivityType.Workout;
                case 12: return ActivityType.Snowboard;
                case 13: return ActivityType.Snowshoe;
                case 14: return ActivityType.Kitesurf;
                case 15: return ActivityType.Windsurf;
                case 16: return ActivityType.Swim;
                case 17: return ActivityType.Virtual_Ride;
                case 18: return ActivityType.EBike_Ride;
                case 19: return ActivityType.Velomobile;
                case 21: return ActivityType.Canoe;
                case 22: return ActivityType.Kayaking;
                case 23: return ActivityType.Rowing;
                case 24: return ActivityType.Stand_Up_Paddling;
                case 25: return ActivityType.Surfing;
                case 26: return ActivityType.Crossfit;
                case 27: return ActivityType.Elliptical;
                case 28: return ActivityType.Rock_Climb;
                case 29: return ActivityType.StairStepper;
                case 30: return ActivityType.Weight_Training;
                case 31: return ActivityType.Yoga;
                case 51: return ActivityType.Handcycle;
                case 52: return ActivityType.Wheelchair;
                case 53: return ActivityType.Virtual_Run;
                default: return ActivityType.None;
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

        /// <summary>
        /// Return a list of latitude and longitude coordinates
        /// </summary>
        /// <returns></returns>
        public List<GPXCoordinates> GetGPXCoordinates()
        {
            //TODO: Refine process to get base trackpoint element and get details from there.
            //      Make no assumption that lat/long/time/elevation exist automatically
            List<XAttribute> latitudesXAtt = gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt", xmlNamespaceManager).Attributes("lat").ToList();
            List<XAttribute> longitudesXAtt = gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt", xmlNamespaceManager).Attributes("lon").ToList();
            List<XElement> timestampsXEle = gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt//p:time", xmlNamespaceManager).ToList();
            List<XElement> elevationsXEle = gpx.XPathSelectElements("//p:gpx//p:trk//p:trkseg//p:trkpt//p:ele", xmlNamespaceManager).ToList();

            List<GPXCoordinates> gPXCoordinates = new List<GPXCoordinates>();
            for (int i = 0; i < latitudesXAtt.Count; i++) //assume that three lists have same XAttributes count
            {
                double latitude = double.Parse(latitudesXAtt[i].Value);
                double longitude = double.Parse(longitudesXAtt[i].Value);
                DateTime timestamp = DateTime.Parse(timestampsXEle[i].Value);
                double elevation = double.Parse(elevationsXEle[i].Value);

                gPXCoordinates.Add(new GPXCoordinates(latitude, longitude, timestamp, elevation));
            }

            return gPXCoordinates;
        }

        /// <summary>
        /// Return a list of elevation to obtain a detailed altimetry
        /// </summary>
        /// <returns></returns>
        public GPXAltimetry GetGPXAltimetry()
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

            return new GPXAltimetry(minElevation, maxElevation, avgElevation, altimetries);
        }
    }
}
