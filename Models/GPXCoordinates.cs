using System;
namespace StravaGPXReaderLib.Models
{
    /// <summary>
    /// Class that define the longitude and latitude properties
    /// </summary>
    public class GPXCoordinates
    {
        public GPXCoordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
