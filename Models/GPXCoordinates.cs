using System;
namespace StravaGPXReaderLib.Models
{
    /// <summary>
    /// Class that define the longitude and latitude properties with additional opttional details.
    /// </summary>
    public class GPXCoordinates
    {
        public GPXCoordinates(double latitude, double longitude, DateTime? timestamp = null, double? elevation = null)
        {
            Latitude = latitude;
            Longitude = longitude;
            Timestamp = timestamp;
            Elevation = elevation;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime? Timestamp { get; set; }
        // Elevation is assumed to be in meters
        public Double? Elevation { get; set; }
    }
}
