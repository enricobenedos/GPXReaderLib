
# GPXReaderLib

This library is with the main purpose to quickly read the main info of GPX file. (Tested only with Garmin's GPX)

![Develop](https://github.com/enricobenedos/GPXReaderLib/workflows/Develop/badge.svg?branch=develop) ![Master](https://github.com/enricobenedos/GPXReaderLib/workflows/Master/badge.svg?branch=master)

How to use it in a console application (in this case .NET 6):
```c#
using System.Xml;
using System.Xml.Linq;
using GPXReaderLib.Interfaces;
using GPXReaderLib.Models;

Console.WriteLine("GpxReaderLib console demonstrative app");

try
{
    XDocument myGPX = XDocument.Load("yourGpxPath");

    XmlNamespaceManager r = new XmlNamespaceManager(new NameTable());
    r.AddNamespace("p", "http://www.topografix.com/GPX/1/1");

    GPXReaderLib.GpxReader gpxReader = new GPXReaderLib.GpxReader(myGPX, r);

    Console.WriteLine("Name: " + gpxReader.GetGpxName());
    Console.WriteLine("Avg Elevation: " + gpxReader.GetElevation(ElevationType.Avg));
    Console.WriteLine("Min Elevation: " + gpxReader.GetElevation(ElevationType.Min));
    Console.WriteLine("Max Elevation: " + gpxReader.GetElevation(ElevationType.Max));
    Console.WriteLine("Start Date: " + gpxReader.GetStartDt());
    Console.WriteLine("End Date: " + gpxReader.GetEndDt());
    Console.WriteLine("Duration: " + gpxReader.GetDuration());
    Console.WriteLine("Distance: " + gpxReader.GetDistance());
    Console.WriteLine("Elevation Gain: " + gpxReader.GetElevationGain());

    Console.WriteLine("Printing complete list of latitude - longitude");

    IEnumerable<TrackPoint> trackPoints = gpxReader.GetGpxCoordinates();
    foreach (var trackPoint in trackPoints)
    {
        Console.WriteLine($"lat: {trackPoint.Latitude} - lon: {trackPoint.Longitude}");
    }

    GpxAltimetry altimetry = gpxReader.GetGpxAltimetry();
    foreach (Altimetry altimetryItem in altimetry.Altimetries)
    {
        Console.WriteLine($"Altimetry value: Meters:{altimetryItem.Elevation} - KM:{altimetryItem.Kilometers}");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.ReadKey();
```
