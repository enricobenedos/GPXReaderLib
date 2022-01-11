using System;
namespace GPXReaderLib.Models
{
    /// <summary>
    /// Contains standard gpx trackpoints information.
    /// </summary>
    public class TrackPoint
    {
        public TrackPoint(double latitude, double longitude, double elevation)
        {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }

        /// <summary>
        /// Trackpoint recorded latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Trackpoint recorded longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Trackpoint recorded elevation.
        /// </summary>
        public double Elevation { get; set; }
    }
}

