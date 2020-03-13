using System;
using System.Collections.Generic;

namespace GPXReaderLib.Models
{
    /// <summary>
    /// Class that define min,average,max altimetry. There is also a list of all recorded altimetries
    /// </summary>
    public class GPXAltimetry
    {
        public GPXAltimetry(double minElevation, double maxElevation, double avgElevation, IEnumerable<double> altimetries)
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
        public IEnumerable<double> Altimetries { get; set; }
    }
}
