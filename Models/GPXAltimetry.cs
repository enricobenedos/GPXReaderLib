using System;
using System.Collections.Generic;

namespace GPXReaderLib.Models
{
    /// <summary>
    /// Class that define min,average,max altimetry. There is also a list of all recorded altimetries
    /// </summary>
    public class GPXAltimetry
    {
        public GPXAltimetry(double minElevation, double maxElevation, double avgElevation, IEnumerable<Altimetry> altimetries)
        {
            MinElevation = minElevation;
            MaxElevation = maxElevation;
            AvgElevation = avgElevation;
            Altimetries = altimetries;
        }

        /// <summary>
        /// Minimum height value
        /// </summary>
        public double MinElevation { get; set; }
        /// <summary>
        /// Maximum height value
        /// </summary>
        public double MaxElevation { get; set; }
        /// <summary>
        /// Average height value
        /// </summary>
        public double AvgElevation { get; set; }
        /// <summary>
        /// List aff all recorded values during navigation
        /// </summary>
        public IEnumerable<Altimetry> Altimetries { get; set; }
    }

    /// <summary>
    /// Define an altimetry record that is composed from elevation value and the relative recorder kilometer
    /// </summary>
    public class Altimetry
    {
        public Altimetry(double elevation, double kilometers)
        {
            Elevation = elevation;
            Kilometers = kilometers;
        }

        /// <summary>
        /// Elevation related to actual kilometer
        /// </summary>
        public double Elevation { get; set; }
        /// <summary>
        /// Actual kilometers value (from 0) when the elevation was recorded
        /// </summary>
        public double Kilometers { get; set; }
    }
}
