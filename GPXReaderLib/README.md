
# GPXReaderLib

This library is created using NET CORE 3.1. The main purpose is to fast read the main info of GPX file. (Tested only with Garmin's GPX)

![Develop](https://github.com/enricobenedos/GPXReaderLib/workflows/Develop/badge.svg?branch=develop) ![Master](https://github.com/enricobenedos/GPXReaderLib/workflows/Master/badge.svg?branch=master)

How to use it in a console application:
```c#
static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            try
            {
                XDocument myGPX = XDocument.Load("myGPXPath");

                XmlNamespaceManager r = new XmlNamespaceManager(new NameTable());
                r.AddNamespace("p", "http://www.topografix.com/GPX/1/1");

                GPXReaderLib.GpxReader gPXReader = new GPXReaderLib.GpxReader(myGPX, r);

                Console.WriteLine("Name: " + gPXReader.GetGPXName());
                Console.WriteLine("Avg Elevation: " + gPXReader.GetElevation(GPXReaderLib.GpxReader.ElevationType.Avg));
                Console.WriteLine("Min Elevation: " + gPXReader.GetElevation(GPXReaderLib.GpxReader.ElevationType.Min));
                Console.WriteLine("Max Elevation: " + gPXReader.GetElevation(GPXReaderLib.GpxReader.ElevationType.Max));
                Console.WriteLine("Start Date: " + gPXReader.GetStartDt());
                Console.WriteLine("End Date: " + gPXReader.GetEndDt());
                Console.WriteLine("Duration: " + gPXReader.GetDuration());
                Console.WriteLine("Distance: " + gPXReader.GetDistance());
                Console.WriteLine("Elevation Gain: " + gPXReader.GetElevationGain());

                Console.WriteLine("Printing complete list of latitude - longitude");

                List<GPXReaderLib.Models.GPXCoordinates> coordinates = gPXReader.GetGPXCoordinates();
                for (int i = 0; i < coordinates.Count; i++)
                {
                    Console.WriteLine($"lat: {coordinates[i].Latitude} - lon: {coordinates[i].Longitude}");
                }

                GPXReaderLib.Models.GPXAltimetry altimetry = gPXReader.GetGPXAltimetry();
                foreach (GPXReaderLib.Models.Altimetry altimetryItem in altimetry.Altimetries)
                {
                    Console.WriteLine($"Altimetry value: Meters:{altimetryItem.Elevation} - KM:{altimetryItem.Kilometers}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
```
