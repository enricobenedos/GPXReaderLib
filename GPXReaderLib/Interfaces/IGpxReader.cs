using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GPXReaderLib.Models;

namespace GPXReaderLib.Interfaces
{
    public enum ElevationType { Min, Max, Avg }

    public interface IGpxReader
    {
        /// <summary>
        /// Get GPX name.
        /// </summary>
        string GetGpxName();

        /// <summary>
        /// Calculate gpx elevation based on given type (min,max, etc..).
        /// </summary>
        /// <param name="elevationType">Specify the elevation type using <see cref="ElevationType"/></param>
        double GetElevation(ElevationType elevationType);

        /// <summary>
        /// Calculate the elevation gain.
        /// </summary>
        /// <param name="treshold">Treshold in kilometers.
        /// This parameter is needed by the algorithm to know the minimum distance between two points
        /// in order to calculate the elevation.
        /// A lower\higher <paramref name="treshold"/> value can cause wrong results.</param>
        /// <returns>Elevation gain in meters</returns>
        double GetElevationGain(double treshold = 0.22);

        /// <summary>
        /// Get gpx start datetime.
        /// </summary>
        DateTime GetStartDt();

        /// <summary>
        /// Get gpx end datetime
        /// </summary>
        DateTime GetEndDt();

        /// <summary>
        /// Calculate the gpx duration.
        /// </summary>
        TimeSpan GetDuration();

        /// <summary>
        /// Calculate total distance in kilometers.
        /// </summary>
        double GetDistance();

        /// <summary>
        /// Create a list that contains ordered latitude and longitude coordinates.
        /// </summary>
        IEnumerable<TrackPoint> GetGpxCoordinates();

        /// <summary>
        /// Generate a list containing the elevation at each kilometer checkpoint.
        /// It also return some altimetry specs as maximum, etc..
        /// </summary>
        GpxAltimetry GetGpxAltimetry();
    }
}

