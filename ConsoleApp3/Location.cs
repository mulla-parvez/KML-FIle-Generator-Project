using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp3
{
    public class Map
    {
        Dictionary<int, List<Location>> _classifiedSectionList;
        public Map()
        {
            _classifiedSectionList = new Dictionary<int, List<Location>>();
        }

        public void Classify(Location locationObj) //group set of locations/points under a single section
        {
            foreach (var section in locationObj._uniqueSections)
            {
                var tempList = new List<Location>();
                foreach (var point in locationObj._allPoints)
                {
                    if (section == point._section)
                        tempList.Add(point);
                }
                if (tempList.Count > 0) _classifiedSectionList.Add(section, tempList);                
            }
            var kmlConverterObj = new KmlConverter();
            kmlConverterObj.ConvertToKML(_classifiedSectionList);
        }
    }

    public class Location
    {
        public string _latitude { get; set; }
        public string _longitude { get; set; }
        public string _description { get; set; }
        public string _name { get; set; }
        public string _icon { get; set; }
        public string _type { get; set; }
        public string _wheel { get; set; }
        public int _section { get; set; }
        public float _DCI { get; set; }
        public List<Location> _allPoints = new List<Location>();
        public List<int> _uniqueSections = new List<int>();

        public void ReadCSV() //reading CSV file to identify attributes of a location/point on the map
        {
            var reader = new StreamReader(@"D:\work\TTI\input.csv");
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Split(',');
                if (!line[0].StartsWith("L"))
                {
                    var locationObj = new Location
                    {
                        _latitude = line[0],
                        _longitude = line[1],
                        _name = line[2],
                        _description = line[3],
                        _icon = line[4],
                        _type = line[5],
                        _wheel=line[6],
                        _section=int.Parse(line[7]),
                        _DCI=float.Parse(line[8])
                    };
                    _allPoints.Add(locationObj);
                    if(_uniqueSections.Count>0)
                    {
                        if (!_uniqueSections.Contains(int.Parse(line[7])))
                            _uniqueSections.Add(int.Parse(line[7]));
                    }
                    else _uniqueSections.Add(int.Parse(line[7]));
                }
            }            
            
        }
    }
}
