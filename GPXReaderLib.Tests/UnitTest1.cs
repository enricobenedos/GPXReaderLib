using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using GPXReaderLib.Interfaces;
using GPXReaderLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPXReaderLib.Tests;

[TestClass]
public class UnitTest1
{
    private readonly GpxReader _gpxReader;
    private readonly double _minElevation = 21.799999237060547;
    private readonly double _avgElevation = 64.69410415369917;
    private readonly double _maxElevation = 182.0;
    private readonly double _distance = 106.73029842820526;

    public UnitTest1()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US");

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
        Assert.AreEqual("Maserada sul Piave Ciclismo", _gpxReader.GetGpxName());
    }

    [TestMethod]
    public void GetMinimumElevation()
    {
        Assert.AreEqual(_minElevation, _gpxReader.GetElevation(ElevationType.Min));
    }

    [TestMethod]
    public void GetAverageElevation()
    {
        Assert.AreEqual(_avgElevation, _gpxReader.GetElevation(ElevationType.Avg));
    }

    [TestMethod]
    public void GetMaximumElevation()
    {
        Assert.AreEqual(_maxElevation, _gpxReader.GetElevation(ElevationType.Max));
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
        Assert.AreEqual(new TimeSpan(3, 56, 55), _gpxReader.GetDuration());
    }

    [TestMethod]
    public void GetDistance()
    {
        double roundedTest = Math.Round(_distance, 4);
        double roundedValue = Math.Round(_gpxReader.GetDistance(), 4);

        Assert.AreEqual(roundedTest, roundedValue);
    }

    [TestMethod]
    public void GetElevationGain()
    {
        double roundedTest = Math.Round(434.799991607666, 4);
        double roundedValue = Math.Round(_gpxReader.GetElevationGain(), 4);

        Assert.AreEqual(roundedTest, roundedValue);
    }

    [TestMethod]
    public void GetMinAltimetry()
    {
        GpxAltimetry gpxAltimetry = _gpxReader.GetGpxAltimetry();

        Assert.AreEqual(_minElevation, gpxAltimetry.MinElevation);
    }

    [TestMethod]
    public void GetMaxAltimetry()
    {
        GpxAltimetry gpxAltimetry = _gpxReader.GetGpxAltimetry();

        Assert.AreEqual(_maxElevation, gpxAltimetry.MaxElevation);
    }

    [TestMethod]
    public void GetAvgAltimetry()
    {
        GpxAltimetry gpxAltimetry = _gpxReader.GetGpxAltimetry();

        Assert.AreEqual(_avgElevation, gpxAltimetry.AvgElevation);
    }

    [TestMethod]
    public void CheckAltimetryCount()
    {
        GpxAltimetry gpxAltimetry = _gpxReader.GetGpxAltimetry();

        Assert.AreEqual(3495, gpxAltimetry.Altimetries.Count());
    }
}
