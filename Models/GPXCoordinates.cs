using System;
namespace GPXReaderLib.Models
{
    /// <summary>
    /// Class that define the longitude and latitude properties
    /// </summary>
    public class GPXCoordinates
    {
        public GPXCoordinates(double latitude, double longitude, double elevation)
        {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
    }
}
