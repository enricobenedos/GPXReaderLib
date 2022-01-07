using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPXReaderLib.Tests;

[TestClass]
public class UnitTest1
{
    private GpxReader _gpxReader;

    public UnitTest1()
    {
        // Load gpx from assets
        XDocument myGPX = XDocument.Load("activity_2970526078.gpx");

        XmlNamespaceManager r = new XmlNamespaceManager(new NameTable());
        r.AddNamespace("p", "http://www.topografix.com/GPX/1/1");

        // initialize reader
        _gpxReader = new GpxReader(myGPX, r);
    }

    [TestMethod]
    public void GetName()
    {
        Assert.AreEqual("Maserada sul Piave Ciclismo", _gpxReader.GetGPXName());
    }

    [TestMethod]
    public void GetMinimumElevation()
    {
        Assert.AreEqual(21.799999237060547, _gpxReader.GetElevation(GpxReader.ElevationType.Min));
    }

    [TestMethod]
    public void GetAverageElevation()
    {
        Assert.AreEqual(64.68675247869096, _gpxReader.GetElevation(GpxReader.ElevationType.Avg));
    }

    [TestMethod]
    public void GetMaximumElevation()
    {
        Assert.AreEqual(182.0, _gpxReader.GetElevation(GpxReader.ElevationType.Max));
    }

    [TestMethod]
    public void GetStartDateTime()
    {
        Assert.AreEqual(DateTime.Parse("2018-08-25T07:57:32.000Z"), _gpxReader.GetStartDt());
    }

    [TestMethod]
    public void GetEndDateTime()
    {
        Assert.AreEqual(DateTime.Parse("2018-08-25T11:54:27.000Z"), _gpxReader.GetEndDt());
    }

    [TestMethod]
    public void GetDuration()
    {
        Assert.AreEqual(new TimeSpan(3,56,55), _gpxReader.GetDuration());
    }

    [TestMethod]
    public void GetDistance()
    {
        Assert.AreEqual(106.73029842820526, _gpxReader.GetDistance());
    }

    [TestMethod]
    public void GetElevationGain()
    {
        Assert.AreEqual(434.799991607666, _gpxReader.GetElevationGain());
    }
}
