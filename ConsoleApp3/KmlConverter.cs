using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp3
{
    public class KmlConverter
    {        
        Document _document;        
        Kml _kml;
        KmlFile _kmlFile;        

        //constructor
        public KmlConverter()
        {
            _document = new Document();
            _kml = new Kml();            
        }

       
        public void ConvertToKML(Dictionary<int, List<Location>> _classifiedSectionList)
        {
            //adding different styles to the document             
            var _leftDipStyle = new Style()
            {
                Balloon = new BalloonStyle() { Text = "$[description]" },
                Id = "leftDipStyle",
                Icon = new IconStyle()
                {
                    Icon = new IconStyle.IconLink(new Uri("http://maps.google.com/mapfiles/kml/paddle/ltblu-blank.png")),
                    Scale = 1.5,
                },
                Label=new LabelStyle() { Scale= 1.5 }
                
            };

            var _rightDipStyle = new Style()
            {
                Balloon = new BalloonStyle() { Text = "$[description]" },
                Id = "rightDipStyle",
                Icon = new IconStyle()
                {
                    Icon = new IconStyle.IconLink(new Uri("http://maps.google.com/mapfiles/kml/paddle/red-blank.png")),
                    Scale = 1.5,
                },
                Label = new LabelStyle() { Scale = 1.5 }
            };

            var _leftBumpStyle = new Style()
            {
                Balloon = new BalloonStyle() { Text = "$[description]" },
                Id = "leftBumpStyle",
                Icon = new IconStyle()
                {
                    Icon = new IconStyle.IconLink(new Uri("http://maps.google.com/mapfiles/kml/paddle/ltblu-circle.png")),
                    Scale = 1.5,
                },
                Label = new LabelStyle() { Scale = 1.5 }
            };

            var _rightBumpStyle = new Style()
            {
                Balloon = new BalloonStyle() { Text = "$[description]"},
                Id = "rightBumpStyle",
                Icon = new IconStyle()
                {
                    Icon = new IconStyle.IconLink(new Uri("http://maps.google.com/mapfiles/kml/paddle/red-circle.png")),
                    Scale=1.5,                    
                },
                Label = new LabelStyle() { Scale = 1.5 }
            };

            var _startSectionStyle = new Style()
            {
                Balloon = new BalloonStyle() { Text = "$[description]" },
                Id = "startSection2",
                Icon = new IconStyle()
                {
                    Icon = new IconStyle.IconLink(new Uri("http://maps.google.com/mapfiles/kml/pushpin/blue-pushpin.png")),
                    Scale = 2.5,
                },
                Label = new LabelStyle() { Scale = 2.5 }
            };

            var _endSectionStyle = new Style()
            {
                Balloon = new BalloonStyle() { Text = "$[description]"},
                Id = "endSection",
                Icon = new IconStyle()
                {
                    Icon = new IconStyle.IconLink(new Uri("http://maps.google.com/mapfiles/kml/pushpin/red-pushpin.png")),
                    Scale = 1.5,
                },
                Label = new LabelStyle() { Scale = 1.5 }
            };            

            var lineStyle1 = new LineStyle { Color = Color32.Parse("ff0000ff"), Width = 2 };
            var roadStyle1 = new Style { Id = "RoadStyle1", Line = lineStyle1 };           

            var lineStyle2 = new LineStyle { Color = Color32.Parse("FFFF0000ff"), Width = 2 };
            var roadStyle2 = new Style { Id = "RoadStyle2", Line = lineStyle2 };            

            _document.AddStyle(_leftDipStyle);
            _document.AddStyle(_rightDipStyle);
            _document.AddStyle(_leftBumpStyle);
            _document.AddStyle(_rightBumpStyle);
            _document.AddStyle(_endSectionStyle);
            _document.AddStyle(roadStyle1);
            _document.AddStyle(roadStyle2);
            
            foreach (var section in _classifiedSectionList) //Looping through different sections
            {
                var _coordinateCollection = new CoordinateCollection();
                int count = 0;                
                foreach (var location in section.Value) //Looping through different Points within each section
                {
                    var _point = new Point  //Marking a point
                    {
                        Coordinate = new Vector(Convert.ToDouble(location._latitude), Convert.ToDouble(location._longitude))
                    };
                    _coordinateCollection.Add(_point.Coordinate);

                    if (count == 0) //to identify if its the starting point of given section
                    {
                        var _placemarkStartSection = new Placemark
                        {
                            Geometry = _point,
                            Description = new Description() { Text = "Start of Section: " + location._section },
                            StyleUrl = new Uri("#startSection2", UriKind.Relative)
                        };
                        _document.AddFeature(_placemarkStartSection);
                    }

                    if(count==section.Value.Count-1) //to identify if its the ending point of given section
                    {
                        var _placemarkEndSection = new Placemark
                        {
                            Geometry = _point,
                            Description = new Description() { Text = "End of Section: " + location._section },
                            StyleUrl = new Uri("#endSection", UriKind.Relative)
                        };
                        _document.AddFeature(_placemarkEndSection);
                    }
                    count++;

                    var _placemark = new Placemark //adding placemark
                    {
                        Geometry = _point,
                        Name = location._name,
                        Description = new Description() { Text = location._description }
                    };
                    //choosing style based on wheel type and type of defect
                    if (location._type == "Dip" && location._wheel == "Left") 
                        _placemark.StyleUrl = new Uri("#leftDipStyle", UriKind.Relative);
                    else if (location._type == "Dip" && location._wheel == "Right")
                        _placemark.StyleUrl = new Uri("#rightDipStyle", UriKind.Relative);
                    else if (location._type == "Bump" && location._wheel == "Left")
                        _placemark.StyleUrl = new Uri("#leftBumpStyle", UriKind.Relative);
                    else
                        _placemark.StyleUrl = new Uri("#rightBumpStyle", UriKind.Relative);
                    _document.AddFeature(_placemark);
                }                
                
                //adding line                
                var _placemark2 = new Placemark()
                {
                    Geometry = new LineString() { Coordinates = _coordinateCollection },
                    Description = new Description() { Text = "Group No.9, DCI: 0.9972, DCI Threshold: 0.70, Need Correction? Yes" }                    
                };
                if (section.Value[0]._DCI > 0.7) //selecting line color based on DCI value
                    _placemark2.StyleUrl = new Uri("#RoadStyle1", UriKind.Relative);
                else
                    _placemark2.StyleUrl = new Uri("#RoadStyle2", UriKind.Relative);
                _document.AddFeature(_placemark2);                //adding line to the document
            }            
            _kml.Feature = _document;

            //create and save KML file
            _kmlFile = KmlFile.Create(_kml, true);           
            using (var stream = File.OpenWrite(@"D:\work\TTI\1.kml"))
            {
                _kmlFile.Save(stream);
            }
        }
    }   
}
