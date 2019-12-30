# GPXReaderLib

This library is created using NET CORE 3.1. The main purpose is to fast read the main info of GPX file. (Tested only with Garmin's GPX)

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

                GPXReaderLib.GPXReader gPXReader = new GPXReaderLib.GPXReader(myGPX, r);

                Console.WriteLine("Name: " + gPXReader.GetGPXName());
                Console.WriteLine("Avg Elevation: " + gPXReader.GetElevation(GPXReaderLib.GPXReader.ElevationType.Avg));
                Console.WriteLine("Min Elevation: " + gPXReader.GetElevation(GPXReaderLib.GPXReader.ElevationType.Min));
                Console.WriteLine("Max Elevation: " + gPXReader.GetElevation(GPXReaderLib.GPXReader.ElevationType.Max));
                Console.WriteLine("Start Date: " + gPXReader.GetStartDt());
                Console.WriteLine("End Date: " + gPXReader.GetEndDt());
                Console.WriteLine("Duration: " + gPXReader.GetDuration());
                Console.WriteLine("Distance: " + gPXReader.GetDistance());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
```